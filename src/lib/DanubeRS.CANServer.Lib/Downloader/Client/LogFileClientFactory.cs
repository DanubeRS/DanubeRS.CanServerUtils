namespace DanubeRS.CANServer.Lib.Downloader.Client;

public class LogFileClientFactory
{
    public static LogFileClient Create(string url)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(url);
        return new LogFileClient(httpClient);
    }
}