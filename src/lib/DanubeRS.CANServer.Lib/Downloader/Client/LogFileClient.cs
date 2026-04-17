using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Polly;
using Polly.Retry;

namespace DanubeRS.CANServer.Lib.Downloader.Client;

using GetLogFilesResponse = (LogFileRecord[] files, bool isEnd);

public interface ILogFileClient
{
    Task<LoggingStatus> GetLoggingStatus(CancellationToken cancellationToken = default);

    Task SetLoggingMode(LoggingMode type, int? interval, int filterInterval,
        CancellationToken cancellationToken = default);

    Task<GetLogFilesResponse> GetLogFiles(long offset = 0, CancellationToken cancellationToken = default);
    Task GetLogFileData(LogFileRecord record, Stream target, CancellationToken cancellationToken = default);
    Task<bool> GetIsAlive(CancellationToken cancellationToken = default);
    Task DeleteFile(LogFileRecord file, CancellationToken cancellationToken = default);
    Task ResetServer(CancellationToken cancellationToken = default);
    Task<LogFileStatus?> GetStats(CancellationToken cancellationToken = default);
}

public class LogFileClient(HttpClient client) : IAsyncDisposable, ILogFileClient
{
    private const string Cont = "---cont---";
    
    private static readonly ResiliencePipeline<object> HttpPipeline = BuildHttpPipeline();

     public async ValueTask DisposeAsync()
    {
        client.Dispose();
        await Task.CompletedTask;
    }
    
    private static ResiliencePipeline<object> BuildHttpPipeline()
    {
        var retryOptions = new RetryStrategyOptions<object>()
        {
            ShouldHandle = new PredicateBuilder<object>().Handle<HttpRequestException>(),
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromMilliseconds(500),
            MaxRetryAttempts = 3,
        };

        return new ResiliencePipelineBuilder<object>()
            .AddRetry(retryOptions)
            .Build();
    }

    public async Task<LoggingStatus> GetLoggingStatus(CancellationToken cancellationToken = default)
    {
        var logStatusResponse = await client.GetAsync("/logs/load", cancellationToken);
        logStatusResponse.EnsureSuccessStatusCode();
        var response = await logStatusResponse.Content.ReadFromJsonAsync<LoggingStatus>(cancellationToken);
        if (response == null)
            throw new NullReferenceException("Unable to get logging status");
        return response;
    }

    public async Task SetLoggingMode(LoggingMode type, int? interval, int filterInterval,
        CancellationToken cancellationToken = default)
    {
        if (type == LoggingMode.Interval && interval == null) throw new ArgumentException("Interval must be specified");
        var response = await client.PostAsync("/logs/save", new FormUrlEncodedContent(new Dictionary<string, string?>()
        {
            { "logmode", type.ToString("D") },
            { "interval", interval?.ToString("D") },
            { "interval_mode3", filterInterval.ToString("D") },
        }), cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetLogFilesResponse> GetLogFiles(long offset = 0, CancellationToken cancellationToken = default)
    {
        var isEnd = true;
        var logFiles =
            await client.GetFromJsonAsync<LogFileRecord[]>("/logs/files?offset=" + offset, cancellationToken);
        if (logFiles == null)
            return ([], true);
        if (logFiles.Any(f => f.Name == Cont))
            isEnd = false;
        return (logFiles.Where(f => f.Name != Cont).ToArray(), isEnd);
    }

    public async Task GetLogFileData(LogFileRecord record, Stream target, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync("/logs/files/download?id=" + Uri.EscapeDataString(record.Name), cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await stream.CopyToAsync(target, cancellationToken);
    }

    public async Task<bool> GetIsAlive(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.GetAsync("/stats", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            return false;
        }
    }

    public async Task DeleteFile(LogFileRecord file, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsync($"/logs/files/delete",
            new FormUrlEncodedContent([new KeyValuePair<string, string>("id", file.Name)]), cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ResetServer(CancellationToken cancellationToken = default)
    {
        await client.GetAsync("/restart", cancellationToken);
    }

    public async Task<LogFileStatus?> GetStats(CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync("/stats", cancellationToken);
        return await response.Content.ReadFromJsonAsync<LogFileStatus>(cancellationToken);
    }
}

public class LogFileStatus
{
    [JsonPropertyName("bus_rx")] public long[] BusRxRate { get; set; }
    [JsonPropertyName("bus_tx")] public long[] BusTxRate { get; set; }
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
