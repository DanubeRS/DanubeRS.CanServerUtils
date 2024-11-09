namespace DanubeRS.CanServerUtils.Lib.Downloader;

public class LogFileClientFactory
{
    public static LogFileClient.LogFileClient Create(string url)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(url);
        return new LogFileClient.LogFileClient(httpClient);
    }
}