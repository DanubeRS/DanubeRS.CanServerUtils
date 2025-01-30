using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DanubeRS.CANServer.Lib.Downloader.Client;

public class LogFileClient : IDisposable
{
    private readonly HttpClient _client;
    private const string Cont = "---cont---";

    public LogFileClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<LoggingStatus> GetLoggingStatus()
    {
        var logStatusResponse = await _client.GetAsync("/logs/load");
        logStatusResponse.EnsureSuccessStatusCode();
        var response = await logStatusResponse.Content.ReadFromJsonAsync<LoggingStatus>();
        if (response == null)
            throw new NullReferenceException("Unable to get logging status");
        return response;
    }

    public async Task SetLoggingMode(LoggingMode type, int? interval, int filterInterval)
    {
        if (type == LoggingMode.Interval && interval == null) throw new ArgumentException("Interval must be specified");
        var response = await _client.PostAsync("/logs/save", new FormUrlEncodedContent(new Dictionary<string, string?>()
        {
            { "logmode", type.ToString("D") },
            { "interval", interval?.ToString("D") },
            { "interval_mode3", filterInterval.ToString("D") },
        }));
        response.EnsureSuccessStatusCode();
    }

    public async Task<(LogFileRecord[] files, bool isEnd)> GetLogFiles(long offset = 0)
    {
        var isEnd = true;
        var logFiles = await _client.GetFromJsonAsync<LogFileRecord[]>("/logs/files?offset=" + offset);
        if (logFiles == null)
            return ([], true);
        if (logFiles.Any(f => f.Name == Cont))
            isEnd = false;
        return (logFiles.Where(f => f.Name != Cont).ToArray(), isEnd);
    }

    public async Task GetLogFileData(LogFileRecord record, Stream target, CancellationToken cancellationToken)
    {
        var logFiles = await _client.GetStreamAsync("/logs/files/download?id=" + record.Name, cancellationToken);
        await logFiles.CopyToAsync(target, cancellationToken);
    }

    public async Task<bool> GetIsAlive(CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync("/stats");
        return response.IsSuccessStatusCode;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task DeleteFile(LogFileRecord file, CancellationToken cancellationToken)
    {
        await _client.PostAsync($"/logs/files/delete",
            new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("id", file.Name) }), cancellationToken);
    }

    public async Task ResetServer(CancellationToken cancellationToken)
    {
        await _client.GetAsync("/restart", cancellationToken);
    }
}

public class LogFileRecord
{
    [JsonPropertyName("n")] public string Name { get; set; }
    [JsonPropertyName("s")] public long Size { get; set; }
}

public class LoggingStatus
{
    [JsonPropertyName("activemode")] public LoggingMode Mode { get; init; }
    [JsonPropertyName("activefile")] public string? ActiveFile { get; init; }
    [JsonPropertyName("interval")] public int Interval { get; init; }
    [JsonPropertyName("totalsize")] public long TotalSpace { get; init; }
    [JsonPropertyName("freespace")] public long FreeSpace { get; init; }
    [JsonPropertyName("interval_mode3")] public int FilterInterval { get; set; }
}

public enum LoggingMode
{
    Disabled = 0,
    Raw = 1,
    Interval = 2,
    Filtered = 3
}

public enum LogFileType
{
    Raw,
    Interval,
    Filtered,
}