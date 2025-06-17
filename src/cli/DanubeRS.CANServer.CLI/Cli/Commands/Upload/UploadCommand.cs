using System.CommandLine;
using System.CommandLine.Parsing;

namespace DanubeRS.CANServer.CLI.Cli.Commands.Upload;

public class UploadCommand : Command
{
    public static UploadCommand Instance { get; } = new();

    public UploadCommand() : base("upload", "Upload CAN log files to InfluxDB")
    {
        AddAlias("ul");

        // Input Options
        AddOption(UploadOptions.InputPath.Instance);

        // DBC Options
        AddOption(UploadOptions.Database.Instance);
        AddOption(UploadOptions.Signals.Instance);

        // Watch Options
        AddOption(UploadOptions.WatchInterval.Instance);
        AddOption(UploadOptions.ArchivePath.Instance);

        // InfluxDB Options
        AddOption(InfluxDbOptions.Endpoint.Instance);
        AddOption(InfluxDbOptions.Org.Instance);
        AddOption(InfluxDbOptions.Bucket.Instance);
        AddOption(InfluxDbOptions.Token.Instance);
        AddOption(InfluxDbOptions.Compress.Instance);
    }
}

public class UploadOptionsValues
{
    public static UploadOptionsValues Parse(ParseResult result)
    {
        return new UploadOptionsValues()
        {
            InputPath = result.GetValueForOption(UploadOptions.InputPath.Instance)!,
            Databases = result.GetValueForOption(UploadOptions.Database.Instance),
            Signals = result.GetValueForOption(UploadOptions.Signals.Instance),
            ArchivePath = result.GetValueForOption(UploadOptions.ArchivePath.Instance),
            WatchInterval = result.GetValueForOption(UploadOptions.WatchInterval.Instance),
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
            Endpoint = result.GetValueForOption(InfluxDbOptions.Endpoint.Instance)!,
            Org = result.GetValueForOption(InfluxDbOptions.Org.Instance)!,
            Bucket = result.GetValueForOption(InfluxDbOptions.Bucket.Instance)!,
            Token = result.GetValueForOption(InfluxDbOptions.Token.Instance)!,
            Compress = result.GetValueForOption(InfluxDbOptions.Compress.Instance),
        };
    }
}

public static class InfluxDbOptions
{
    public sealed class Endpoint : Option<Uri>
    {
        public Endpoint() : base("influx-server", "InfluxDB server endpoint")
        {
            IsRequired = true;
        }

        public static Endpoint Instance { get; } = new();
    }

    public sealed class Org : Option<string>
    {
        public Org() : base("influx-org", "InfluxDB organization")
        {
            IsRequired = false;
        }

        public static Org Instance { get; } = new();
    }

    public sealed class Bucket : Option<string>
    {
        public Bucket() : base("influx-bucket", "InfluxDB bucket")
        {
            IsRequired = false;
        }

        public static Bucket Instance { get; } = new();
    }

    public sealed class Token : Option<string>
    {
        public Token() : base("influx-token", "InfluxDB token")
        {
            IsRequired = false;
        }

        public static Token Instance { get; } = new();
    }

    public sealed class Compress : Option<bool>
    {
        public Compress() : base("influx-compress", "Compress data before sending to InfluxDB")
        {
            IsRequired = false;
            SetDefaultValue(false);
        }

        public static Compress Instance { get; } = new();
    }
}

public static class UploadOptions
{
    public sealed class InputPath : Option<DirectoryInfo>
    {
        public static InputPath Instance { get; } = new();

        private InputPath() : base("input", "Path to the directory containing the CAN log files")
        {
            IsRequired = true;
        }
    }

    public sealed class ArchivePath : Option<FileInfo?>
    {
        public static ArchivePath Instance { get; } = new();

        private ArchivePath() : base("archive", "Path to the archive file once successfully parsed and uploaded")
        {
            IsRequired = false;
        }
    }

    public sealed class WatchInterval : Option<int>
    {
        public static WatchInterval Instance { get; } = new();

        private WatchInterval() : base("watch", "Interval in seconds to watch for new files")
        {
            IsRequired = false;
            SetDefaultValue(60);
        }
    }

    public sealed class Signals : Option<uint[]?>
    {
        public static Signals Instance { get; } = new();

        private Signals() : base("signals", "Signal(s) in int format to upload to the database")
        {
            Arity = ArgumentArity.ZeroOrMore;
            IsRequired = false;
        }
    }

    public sealed class Database : Option<FileInfo[]>
    {
        public static Database Instance { get; } = new();

        private Database() : base("dbc", "DBC reference database for signals")
        {
            IsRequired = true;
            Arity = ArgumentArity.OneOrMore;
        }
    }
}