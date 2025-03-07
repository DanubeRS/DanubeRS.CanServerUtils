using System.Diagnostics;
using System.Net;
using DanubeRS.CANServer.Lib.Downloader.Client;
using Humanizer;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace DanubeRS.CANServer.Lib.Downloader;

/// <summary>
/// Downloader facade for listing, iterating, downloading, and optionally deleting files from a CANServer SD Card
/// </summary>
/// <param name="url"></param>
/// <param name="logger"></param>
public class Downloader(string url, ILogger<Downloader> logger)
{
    private LogFileClient CreateClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(url);
        return new LogFileClient(httpClient);
    }

    public async Task<int> DownloadAllFiles(LogFileType type, bool deleteOnDownload = false, bool implicitLogDisable = false,
        string? outputPath = null, CancellationToken cancellationToken = default,
        Action<string>? onFileDownloaded = null, Func<IEnumerable<LogFileRecord>, LogFileClient, Task<bool>>? shouldDownloadAllFilesTest = null,
        Func<LogFileRecord, LogFileClient, Task<(bool skip, bool skipAll)>>? shouldDownloadFileTest = null)
    {
        using var client = CreateClient();
        bool isAlive = false;
        try
        {
            isAlive = await client.GetIsAlive(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get alive status, assuming false...");
        }
        if (!isAlive)
        {
            logger.LogWarning("Client is not alive. Returning");
            return 0;
        }

        var allFiles = await GetStoredFiles(client);

        if (shouldDownloadAllFilesTest != null && !await shouldDownloadAllFilesTest.Invoke(allFiles, client))
        {
            logger.LogDebug("Download files test has failed, so not downloading anything...");
            return 0;
        }

        var logStatus = await client.GetLoggingStatus();
        try
        {
            if (logStatus.Mode == LoggingMode.Disabled)
            {
                logger.LogDebug("Logger is already disabled. Continuing...");
            }
            else if (logStatus.Mode != LoggingMode.Disabled && implicitLogDisable)
            {
                await client.SetLoggingMode(LoggingMode.Disabled, logStatus.Interval, logStatus.FilterInterval);
            }
            else
            {
                throw new DownloaderException("Logging is not disabled before download");
            }

            return await DownloadAllFilesInternal(client, deleteOnDownload, outputPath, cancellationToken, onFileDownloaded, shouldDownloadFileTest);
        }
        finally
        {
            if (logStatus.Mode != LoggingMode.Disabled)
            {
                logger.LogDebug("Logging was implicitly disabled. Re-enabling");
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

    private async Task<int> DownloadAllFilesInternal(LogFileClient client, bool deleteOnDownload,
        string? outputPath, CancellationToken cancellationToken = default, Action<string>? onDownloaded = null,
        Func<LogFileRecord, LogFileClient, Task<(bool skip, bool skipAll)>>? shouldDownloadFileTest = null,
        IEnumerable<LogFileRecord>? allFiles = null)
    {
        var downloadDate = DateTimeOffset.UtcNow;
        allFiles ??= await GetStoredFiles(client);
        var sw = new Stopwatch();
        var outputDir = outputPath == null
            ? new DirectoryInfo(Directory.GetCurrentDirectory())
            : new DirectoryInfo(outputPath.Replace("~",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        if (!outputDir.Exists)
        {
            outputDir.Create();
        }

        var downloadCount = 0;
        foreach (var file in allFiles.OrderBy(f => f.Name))
        {
            if (shouldDownloadFileTest != null)
            {
                var (skip, skipAll) = await shouldDownloadFileTest.Invoke(file, client);
                if (skipAll)
                {
                    logger.LogInformation("All remaining files should be skipped...");
                    return downloadCount;
                }
                if (skip)
                {
                    logger.LogInformation("File {file} should be skipped...", file.Name);
                    continue;
                }
            }
            
            var notFoundRetry = new RetryStrategyOptions<string?>()
            {
                ShouldHandle =
                    new PredicateBuilder<string?>().Handle<HttpRequestException>(exception =>
                            exception.StatusCode == HttpStatusCode.NotFound)
                        .Handle<TimeoutRejectedException>(),
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromSeconds(10),
                OnRetry = async args =>
                {
                    logger.LogWarning("Server was unable to find the file, suggesting that it needs to be restarted");
                    logger.LogInformation("Restarting server. Attempt {Attempt}", args.AttemptNumber);
                    await client.ResetServer(args.Context.CancellationToken);
                    var isAliveLoop = new RetryStrategyOptions<bool>()
                    {
                        Delay = TimeSpan.FromSeconds(1),
                        BackoffType = DelayBackoffType.Exponential,
                        ShouldHandle = arguments => ValueTask.FromResult(!arguments.Outcome.Result),
                    };
                    await new ResiliencePipelineBuilder<bool>().AddRetry(isAliveLoop).Build().ExecuteAsync(
                        async (clientState, token) => await clientState.GetIsAlive(token),
                        client,
                        args.Context.CancellationToken).ConfigureAwait(false);
                    logger.LogInformation("Server successfully restarted. Downloading file again");
                },
                MaxRetryAttempts = 3,
            };

            var badFileSizeRetry = new RetryStrategyOptions<string?>()
            {
                ShouldHandle = new PredicateBuilder<string?>().HandleResult(s => s == null),
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.Zero,
                MaxRetryAttempts = 3,
                OnRetry = ctx =>
                {
                    logger.LogWarning(
                        "Returned file was not the expected size. Downloading again (attempt {Attempts})...",
                        ctx.AttemptNumber);
                    return ValueTask.CompletedTask;
                },
            };

            try
            {
                var archiveFile = await new ResiliencePipelineBuilder<string?>().AddRetry(notFoundRetry)
                    .AddTimeout(TimeSpan.FromMinutes(5))
                    .AddRetry(badFileSizeRetry).Build().ExecuteAsync(async (clientState, token) =>
                    {
                        logger.LogInformation("Downloading file {File}", file.Name);
                        var archiveFile = Path.Combine(outputDir.FullName, $"{downloadDate:yyyyddMHHmmss}_{file.Name}");
                        await using var ms = new MemoryStream();
                        sw.Restart();
                        await clientState.GetLogFileData(file, ms, token);
                        var time = sw.Elapsed;
                        logger.LogDebug("Downloaded {File} ({Bytes}) in {Seconds}s ({Rate}/s)",
                            file.Name,
                            file.Size.Bytes().Humanize(),
                            time.TotalSeconds, (file.Size / time.TotalSeconds).Bytes().Humanize("000.00"));
                        // The file length does not match what we expected
                        if (file.Size != ms.Length) return null;
                        ms.Seek(0, SeekOrigin.Begin);
                        // File successfully read to mem, now write to file on disk!
                        await using var fs =
                            new FileStream(archiveFile,
                                FileMode.CreateNew);
                        await ms.CopyToAsync(fs, token);
                        return archiveFile;
                    }, client, cancellationToken);

                if (archiveFile == null)
                    throw new Exception("Unable to download file. Returned file was not the expected size");


                onDownloaded?.Invoke(archiveFile);
                if (deleteOnDownload)
                {
                    try
                    {
                        logger.LogInformation("Deleting source file {file}", file.Name);
                        await client.DeleteFile(file, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to delete log file");
                        continue;
                    }
                }
                
                downloadCount++;
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
            {
                logger.LogInformation(e, "Operation cancelled");
                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to download log file");
            }
        }

        return downloadCount;
    }

    private static async Task<IReadOnlyCollection<LogFileRecord>> GetStoredFiles(LogFileClient client)
    {
        var allFiles = new List<LogFileRecord>();
        long offset = 0;
        bool isEnd;
        do
        {
            (var files, isEnd) = await client.GetLogFiles(offset);
            allFiles.AddRange(files);
            offset += files.Length;
        } while (!isEnd);

        return allFiles;
    }

    private class DownloaderException(string message) : Exception(message)
    {
    }
}