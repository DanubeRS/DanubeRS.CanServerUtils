using System.CommandLine;

namespace DanubeRS.CANServer.CLI.Commands.Upload;

public class UploadCommand : Command
{
    public static UploadCommand Instance { get; } = new();

    public UploadCommand() : base("upload", "Parse CAN log files and upload decoded signals to InfluxDB")
    {
        Aliases.Add("ul");

        Options.Add(UploadOptions.InputPath.Instance);
        Options.Add(UploadOptions.Database.Instance);
        Options.Add(UploadOptions.Signals.Instance);
        Options.Add(UploadOptions.WatchInterval.Instance);
        Options.Add(UploadOptions.ArchivePath.Instance);
        Options.Add(InfluxDbOptions.Endpoint.Instance);
        Options.Add(InfluxDbOptions.Org.Instance);
        Options.Add(InfluxDbOptions.Bucket.Instance);
        Options.Add(InfluxDbOptions.Token.Instance);
        Options.Add(InfluxDbOptions.Compress.Instance);
    }
}

public class UploadOptionsValues
{
    public static UploadOptionsValues Parse(ParseResult result)
    {
        return new UploadOptionsValues()
        {
            InputPath = result.GetValue(UploadOptions.InputPath.Instance)!,
            Databases = result.GetValue(UploadOptions.Database.Instance),
            Signals = result.GetValue(UploadOptions.Signals.Instance),
            ArchivePath = result.GetValue(UploadOptions.ArchivePath.Instance),
            WatchInterval = result.GetValue(UploadOptions.WatchInterval.Instance),
            InfluxDb = InfluxDbOptionsValues.Parse(result),
        };
    }

    public DirectoryInfo InputPath { get; private set; } = null!;
    public FileInfo[]? Databases { get; private set; }
    public uint[]? Signals { get; private set; }
    public FileInfo? ArchivePath { get; private set; }
    public int? WatchInterval { get; private set; }
    public InfluxDbOptionsValues InfluxDb { get; private set; } = null!;
}

public class InfluxDbOptionsValues
{
    public Uri Endpoint { get; private set; } = null!;
    public string? Org { get; private set; }
    public string? Bucket { get; private set; }
    public string? Token { get; private set; }
    public bool Compress { get; private set; }

    public static InfluxDbOptionsValues Parse(ParseResult result)
    {
        return new InfluxDbOptionsValues()
        {
            Endpoint = result.GetValue(InfluxDbOptions.Endpoint.Instance)!,
            Org = result.GetValue(InfluxDbOptions.Org.Instance)!,
            Bucket = result.GetValue(InfluxDbOptions.Bucket.Instance)!,
            Token = result.GetValue(InfluxDbOptions.Token.Instance)!,
            Compress = result.GetValue(InfluxDbOptions.Compress.Instance),
        };
    }
}

public static class InfluxDbOptions
{
    public sealed class Endpoint : Option<Uri>
    {
        public static Endpoint Instance { get; } = new();

        private Endpoint() : base("--influxdb-server")
        {
            Description = "URL of the InfluxDB 2.x server (e.g. https://influx.example.com)";
            HelpName = "URL";
            Required = true;
            CustomParser = OptionParsers.UriParser;
        }
    }

    public sealed class Org : Option<string>
    {
        public static Org Instance { get; } = new();

        private Org() : base("--influxdb-org")
        {
            Description = "InfluxDB organization name that owns the target bucket";
            HelpName = "NAME";
        }
    }

    public sealed class Bucket : Option<string>
    {
        public static Bucket Instance { get; } = new();

        private Bucket() : base("--influxdb-bucket")
        {
            Description = "InfluxDB bucket to write decoded CAN signal data into";
            HelpName = "NAME";
        }
    }

    public sealed class Token : Option<string>
    {
        public static Token Instance { get; } = new();

        private Token() : base("--influxdb-token")
        {
            Description = "InfluxDB API token with write access to the target bucket";
            HelpName = "TOKEN";
        }
    }

    public sealed class Compress : Option<bool>
    {
        public static Compress Instance { get; } = new();

        private Compress() : base("--influxdb-compress")
        {
            Description = "Enable gzip compression on write requests to reduce network bandwidth";
            DefaultValueFactory = _ => false;
        }
    }
}

public static class UploadOptions
{
    public sealed class InputPath : Option<DirectoryInfo>
    {
        public static InputPath Instance { get; } = new();

        private InputPath() : base("--input", "-i")
        {
            Description = "Directory containing .log files to parse and upload";
            HelpName = "DIR";
            Required = true;
        }
    }

    public sealed class ArchivePath : Option<FileInfo?>
    {
        public static ArchivePath Instance { get; } = new();

        private ArchivePath() : base("--archive")
        {
            Description = "Move successfully processed log files to this archive file after upload";
            HelpName = "FILE";
        }
    }

    public sealed class WatchInterval : Option<int>
    {
        public static WatchInterval Instance { get; } = new();

        private WatchInterval() : base("--watch")
        {
            Description = "Poll interval in seconds to check for new log files. Set to 0 for a single pass without polling";
            HelpName = "SECONDS";
            DefaultValueFactory = _ => 60;
        }
    }

    public sealed class Signals : Option<uint[]?>
    {
        public static Signals Instance { get; } = new();

        private Signals() : base("--signals")
        {
            Description = "Filter to only upload signals from specific CAN frame IDs. Can be specified multiple times (e.g. --signals 257 --signals 512)";
            HelpName = "FRAME_ID";
            Arity = ArgumentArity.ZeroOrMore;
        }
    }

    public sealed class Database : Option<FileInfo[]>
    {
        public static Database Instance { get; } = new();

        private Database() : base("--dbc")
        {
            Description = "One or more DBC database files used to decode CAN frame payloads into named signals";
            HelpName = "FILE";
            Required = true;
            Arity = ArgumentArity.OneOrMore;
        }
    }
}
