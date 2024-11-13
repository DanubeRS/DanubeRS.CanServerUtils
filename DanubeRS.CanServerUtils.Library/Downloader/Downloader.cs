using System.Diagnostics;
using DanubeRS.CanServerUtils.Lib.LogFileClient;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CanServerUtils.Lib.Downloader;

public class Downloader(string url, ILogger<Downloader> logger)
{
    private LogFileClient.LogFileClient CreateClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(url);
        return new LogFileClient.LogFileClient(httpClient);
    }

    public async Task DownloadAllFiles(LogFileType type, bool deleteOnDownload = false, bool implicitLogDisable = false,
        string? outputPath = null)
    {
        using var client = CreateClient();
        var isAlive = await client.GetIsAlive();
        if (!isAlive)
        {
            logger.LogInformation("Client is not alive. Returning");
            return;
        }

        var logStatus = await client.GetLoggingStatus();
        try
        {
            if (logStatus.Mode == LoggingMode.Disabled)
            {
                logger.LogInformation("Logger is already disabled. Continuing...");
            }
            else if (logStatus.Mode != LoggingMode.Disabled && implicitLogDisable)
            {
                await client.SetLoggingMode(LoggingMode.Disabled, logStatus.Interval, logStatus.FilterInterval);
            }
            else
            {
                throw new DownloaderException("Logging is not disabled before download");
            }

            await DownloadAllFilesInternal(client, deleteOnDownload, outputPath);
        }
        finally
        {
            if (logStatus.Mode != LoggingMode.Disabled)
            {
                logger.LogInformation("Logging was implicitly disabled. Re-enabling");
                try
                {
                    await client.SetLoggingMode(logStatus.Mode, logStatus.Interval, logStatus.FilterInterval);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Unable to re-enable logging");
                }
            }
        }
    }

    private async Task DownloadAllFilesInternal(LogFileClient.LogFileClient client, bool deleteOnDownload,
        string? outputPath, CancellationToken cancellationToken = default)
    {
        var allFiles = new List<LogFileRecord>();
        var downloadDate = DateTimeOffset.UtcNow;

        long offset = 0;

        bool isEnd;
        do
        {
            (var files, isEnd) = await client.GetLogFiles(offset);
            allFiles.AddRange(files);
            offset += files.Length;
        } while (!isEnd);

        var sw = new Stopwatch();
        var outputDir = outputPath == null
            ? new DirectoryInfo(Directory.GetCurrentDirectory())
            : new DirectoryInfo(outputPath.Replace("~",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        if (!outputDir.Exists)
        {
            outputDir.Create();
        }

        foreach (var file in allFiles.OrderBy(f => f.Name))
        {
            logger.LogInformation("Downloading file {File}", file.Name);

            await using var ms =
                new FileStream(Path.Combine(outputDir.FullName, $"{downloadDate:yyyyddMHHmmss}_{file.Name}"),
                    FileMode.CreateNew);
            sw.Restart();
            await client.GetLogFileData(file, ms, cancellationToken);
            var time = sw.Elapsed;
            logger.LogInformation("Downloaded {Bytes} in {Seconds}s ({Rate}/s)", file.Size.Bytes().Humanize(),
                time.TotalSeconds, (file.Size / time.TotalSeconds).Bytes().Humanize("000.00"));
        }
    }

    private class DownloaderException(string message) : Exception(message)
    {
    }
}