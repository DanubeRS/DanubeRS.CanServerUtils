using System.Runtime.Serialization;
using DanubeRS.CanServerUtils.Lib.LogFileClient;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CanServerUtils.Lib.Downloader;

public class Downloader(string url, ILogger<Downloader> logger)
{
    public LogFileClient.LogFileClient Client { get; } = new LogFileClient.LogFileClient(new HttpClient());

    public async Task DownloadAllFiles(LogFileType type, bool deleteOnDownload = false, bool implicitLogDisable = false)
    {
        var isAlive = await Client.GetIsAlive();
        if (!isAlive)
        {
            logger.LogInformation("Client is not alive. Returning");
            return;
        }

        var logStatus = await Client.GetLoggingStatus();
        if (logStatus.Mode != LoggingMode.Disabled && implicitLogDisable)
        {
            await Client.SetLoggingMode(LoggingMode.Disabled, logStatus.Interval, logStatus.FilterInterval);
        }
        else
        {
            throw new DownloaderException("Logging is not disabled before download");
        }
    }

    private class DownloaderException(string message) : Exception(message)
    {
    }
}