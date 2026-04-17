using System.CommandLine;
using DanubeRS.CANServer.Lib.Downloader;
using DanubeRS.CANServer.Lib.Downloader.Client;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Commands.Download;

public class DownloadCommandHandler(DownloaderFactory downloaderFactory, ILogger<DownloadCommandHandler> logger)
    : ICommandAction
{
    private const int MaxConsecutiveErrors = 5;
    private const int FileCountThreshold = 1;

    public async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var options = DownloadOptionValues.Parse(parseResult);
        var downloader = downloaderFactory(options.Endpoint);

        var consecutiveErrors = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await DownloadInternal(downloader, options,
                    async (records, client) => ShouldDownloadAllFilesBasedOnCount(records) &&
                                                 !(options.KillWhenAlive && await BusIsActive(client, cancellationToken)),
                    async (_, client) =>
                        new(false, options.KillWhenAlive && await BusIsActive(client, cancellationToken)),
                    null, cancellationToken);
                consecutiveErrors = 0;
            }
            catch (Exception ex)
            {
                consecutiveErrors++;
                logger.LogError(ex, "Error occurred in loop");
                if (consecutiveErrors >= MaxConsecutiveErrors)
                {
                    logger.LogError("Exceeded {MaxErrors} consecutive errors, exiting", MaxConsecutiveErrors);
                    return 1;
                }
            }

            if (options.Watch <= 0)
                break;
            logger.LogInformation("Successfully performed a download loop. Waiting {delay}s before next check",
                options.Watch);
            await Task.Delay(TimeSpan.FromSeconds(options.Watch), cancellationToken);
        }

        return 0;
    }

    private bool ShouldDownloadAllFilesBasedOnCount(IEnumerable<LogFileRecord> files)
    {
        var fileCount = files?.Count() ?? 0;
        if (fileCount <= FileCountThreshold)
        {
            logger.LogInformation(
                "{FileCount} files found, below threshold ({FileCountThreshold}) - skipping download",
                fileCount, FileCountThreshold);
            return false;
        }

        logger.LogInformation(
            "{FileCount} files found, above threshold ({FileCountThreshold}) - proceeding with download",
            fileCount, FileCountThreshold);
        return true;
    }

    // Note: This method has a side effect — it re-enables logging if the bus is active but logging was disabled
    async Task<bool> BusIsActive(ILogFileClient client, CancellationToken cancellationToken)
    {
        var stats = await client.GetStats(cancellationToken);
        if (stats == null) return false;
        if (stats.BusRxRate.Sum() > 0)
        {
            logger.LogInformation("Bus is active");
            var clientLogStatus = await client.GetLoggingStatus(cancellationToken);
            if (clientLogStatus.Mode == LoggingMode.Disabled)
            {
                logger.LogInformation("Bus is active, but logging was disabled. Re-enabling raw.");
                await client.SetLoggingMode(LoggingMode.Raw, 200, 200, cancellationToken);
            }

            return true;
        }

        logger.LogDebug("Bus is inactive");
        return false;
    }

    async Task<int> DownloadInternal(Downloader downloader,
        DownloadOptionValues options,
        ShouldDownloadAllFilesTest? shouldDownloadAllFilesTest = null,
        ShouldDownloadFileTest? shouldDownloadFileTest = null,
        OnFileDownloaded? onFileDownloaded = null,
        CancellationToken cancellationToken = default, bool forceRemove = false)
    {
        return await downloader.DownloadAllFiles(LogFileType.Interval, options.Remove || forceRemove, true,
            options.OutputPath,
            cancellationToken, shouldDownloadFileTest, shouldDownloadAllFilesTest, onFileDownloaded);
    }
}
