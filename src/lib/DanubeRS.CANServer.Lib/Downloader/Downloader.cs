using System.Diagnostics;
using System.Net;
using DanubeRS.CANServer.Lib.Downloader.Client;
using Humanizer;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace DanubeRS.CANServer.Lib.Downloader;

public record ShouldDownloadFileTestResult(bool Skip, bool SkipAll);

public delegate Downloader DownloaderFactory(Uri uri);

public delegate Task<bool> ShouldDownloadAllFilesTest(IEnumerable<LogFileRecord> files, ILogFileClient client);

public delegate Task<ShouldDownloadFileTestResult> ShouldDownloadFileTest(LogFileRecord file, ILogFileClient client);

public delegate Task OnFilesDownloaded(IEnumerable<string> files, ILogFileClient client);

public delegate Task OnFileDownloaded(string file, ILogFileClient client);

/// <summary>
/// Downloader facade for listing, iterating, downloading, and optionally deleting files from a CANServer SD Card
/// </summary>
/// <param name="url"></param>
/// <param name="logger"></param>
public class Downloader(ILogFileClient client, ILogger<Downloader> logger)
{
    public async Task<int> DownloadAllFiles(LogFileType type, bool deleteOnDownload = false,
        bool implicitLogDisable = false,
        DirectoryInfo? outputPath = null, CancellationToken cancellationToken = default,
        ShouldDownloadFileTest? shouldDownloadFileTest = null,
        ShouldDownloadAllFilesTest? shouldDownloadAllFilesTest = null,
        OnFileDownloaded? onFileDownloaded = null,
        OnFilesDownloaded? onFilesDownloaded = null)
    {
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

        var allFiles = await GetStoredFiles();

        if (shouldDownloadAllFilesTest != null && !await shouldDownloadAllFilesTest.Invoke(allFiles, client))
        {
            logger.LogDebug("Download files test has failed, so not downloading anything...");
            return 0;
        }

        var logStatus = await client.GetLoggingStatus(cancellationToken);
        try
        {
            if (logStatus.Mode == LoggingMode.Disabled)
            {
                logger.LogDebug("Logger is already disabled. Continuing...");
            }
            else if (logStatus.Mode != LoggingMode.Disabled && implicitLogDisable)
            {
                await client.SetLoggingMode(LoggingMode.Disabled, logStatus.Interval, logStatus.FilterInterval,
                    cancellationToken);
            }
            else
            {
                throw new DownloaderException("Logging is not disabled before download");
            }

            return await DownloadAllFilesInternal(deleteOnDownload, outputPath, cancellationToken,
                onFileDownloaded, shouldDownloadFileTest);
        }
        finally
        {
            if (logStatus.Mode != LoggingMode.Disabled)
            {
                logger.LogDebug("Logging was implicitly disabled. Re-enabling");
                try
                {
                    await client.SetLoggingMode(logStatus.Mode, logStatus.Interval, logStatus.FilterInterval,
                        cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Unable to re-enable logging");
                }
            }
        }
    }

    private async Task<int> DownloadAllFilesInternal(bool deleteOnDownload,
        DirectoryInfo? outputPath, CancellationToken cancellationToken = default, OnFileDownloaded? onDownloaded = null,
        ShouldDownloadFileTest? shouldDownloadFileTest = null,
        IEnumerable<LogFileRecord>? allFiles = null)
    {
        var downloadDate = DateTimeOffset.UtcNow;
        allFiles ??= await GetStoredFiles();
        var sw = new Stopwatch();
        var outputDir = outputPath == null
            ? new DirectoryInfo(Directory.GetCurrentDirectory())
            : outputPath;
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
                var archiveFile = await new ResiliencePipelineBuilder<string?>()
                    .AddRetry(notFoundRetry)
                    .AddTimeout(TimeSpan.FromMinutes(5))
                    .AddRetry(badFileSizeRetry)
                    .Build()
                    .ExecuteAsync(async (clientState, token) =>
                    {
                        logger.LogInformation("Downloading file {File}", file.Name);
                        var archiveFile = Path.Combine(outputDir.FullName, $"{downloadDate:yyMMdd_hhmmss}_{file.Name}");
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


                onDownloaded?.Invoke(archiveFile, client);
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

    private async Task<IReadOnlyCollection<LogFileRecord>> GetStoredFiles()
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