using CommandLine;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace DanubeRS.CANServer.CLI;

public interface IDownloadOptions
{
    [Option('s', "source", Required = true, HelpText = "CAN server address")]
    string Address { get; set; }

    [Option('o', "output", Required = true, HelpText = "Path to store downloaded files")]
    string OutputPath { get; set; }

    [Option("rm", Required = false, HelpText = "Remove files from CANServer after download", Default = false)]
    bool Remove { get; set; }
}

public interface IParseOptions
{
    [Option('d', "database", Required = true, HelpText = "Database files to use for message decoding")]
    IEnumerable<string> Databases { get; set; }
    
    [Option('s', "signal", Required = false, HelpText = "ID of the signal(s) to filter out", Default = null)]
    IEnumerable<uint>? Signals { get; set; }
}

public interface IUploadOptions
{
    [Option("archive", Required = false, HelpText = "Archive Directory")]
    string? ArchivePath { get; set; }

    [Option("watch", Required = false, HelpText = "Watch, with delay (in seconds)")]
    public int Watch { get; set; }

    [Option("influxServer", Required = true, HelpText = "InfluxDB Server to upload to")]
    string InfluxAddress { get; set; }

    [Option("influxOrg", Required = false, HelpText = "InfluxDB Bucket to upload to")]
    string InfluxOrg { get; set; }

    [Option("influxBucket", Required = false, HelpText = "InfluxDB Bucket to upload to")]
    string InfluxBucket { get; set; }

    [Option("influxToken", Required = false, HelpText = "InfluxDB Key to upload with")]
    string InfluxToken { get; set; }

    [Option("influxCompress", Required = false, HelpText = "InfluxDB Compress File", Default = false)]
    bool InfluxCompress { get; set; }
}

public interface IParseFromFilesystemOptions : IParseOptions
{
    [Option('i', "input", Required = false, HelpText = "Remove files after download")]
    public string? InputPath { get; set; }
}

[Verb("parse")]
public class ParseOptions : IParseFromFilesystemOptions, IUploadOptions
{
    public IEnumerable<string> Databases { get; set; } = [];
    public IEnumerable<uint>? Signals { get; set; }
    public string? ArchivePath { get; set; }
    public int Watch { get; set; } = 0;
    public required string InfluxAddress { get; set; }
    public required string InfluxOrg { get; set; }
    public required string InfluxBucket { get; set; }
    public required string InfluxToken { get; set; }
    public bool InfluxCompress { get; set; }
    public string? InputPath { get; set; }
}

[Verb("downloadAndParse")]
public class DownloadAndParseOptions : IDownloadOptions, IParseOptions, IUploadOptions
{
    public string Address { get; set; }
    public string OutputPath { get; set; }
    public bool Remove { get; set; }
    public string? InputPath { get; set; }
    public IEnumerable<string> Databases { get; set; }
    public IEnumerable<uint>? Signals { get; set; }
    public string? ArchivePath { get; set; }
    public int Watch { get; set; }
    public string InfluxAddress { get; set; }
    public string InfluxOrg { get; set; }
    public string InfluxBucket { get; set; }
    public string InfluxToken { get; set; }
    public bool InfluxCompress { get; set; }
}

[Verb("download", false, new[] { "dl", "d" }, HelpText = "Downloads CAN server log files")]
public class DownloadOptions : IDownloadOptions
{
    public required string Address { get; set; }
    public required string OutputPath { get; set; }
    public bool Remove { get; set; } = false;
}

[Verb("continuousDownloadAndParse", false, new[] { "cdp" },
    HelpText =
        "Continuously downloads, parses, and stored logs; but only when the CANServer is not actively recieving")]
public class ContinuousDownloadAndParseOptions : IDownloadOptions, IParseOptions, IUploadOptions
{
    public string Address { get; set; }
    public string OutputPath { get; set; }
    public bool Remove { get; set; }
    public string? InputPath { get; set; }
    public IEnumerable<string> Databases { get; set; }
    public IEnumerable<uint>? Signals { get; set; }
    public string? ArchivePath { get; set; }
    public int Watch { get; set; }
    public string InfluxAddress { get; set; }
    public string InfluxOrg { get; set; }
    public string InfluxBucket { get; set; }
    public string InfluxToken { get; set; }
    public bool InfluxCompress { get; set; }
}