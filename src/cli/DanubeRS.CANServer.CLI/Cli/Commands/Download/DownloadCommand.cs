using System.CommandLine;
using System.CommandLine.Invocation;

namespace DanubeRS.CANServer.CLI.Cli.Commands.Download;

public class DownloadCommand : Command
{
    public static DownloadCommand Instance { get; } = new();

    private DownloadCommand() : base("download", "Download CAN log files from CANServer")
    {
        AddAlias("dl");
        AddOption(DownloadOptions.Endpoint.Instance);
        AddOption(DownloadOptions.Output.Instance);
        AddOption(DownloadOptions.Watch.Instance);
        AddOption(DownloadOptions.Remove.Instance);
        AddOption(DownloadOptions.KillWhenAlive.Instance);
    }
}

public class DownloadOptionValues
{
    public static DownloadOptionValues Parse(InvocationContext context)
    {
        return new DownloadOptionValues
        {
            Endpoint = context.ParseResult.GetValueForOption(DownloadOptions.Endpoint.Instance)!,
            OutputPath = context.ParseResult.GetValueForOption(DownloadOptions.Output.Instance),
            Watch = context.ParseResult.GetValueForOption(DownloadOptions.Watch.Instance),
            Remove = context.ParseResult.GetValueForOption(DownloadOptions.Remove.Instance),
            KillWhenAlive = context.ParseResult.GetValueForOption(DownloadOptions.KillWhenAlive.Instance),
        };
    }

    public Uri Endpoint { get; private set; } = null!;
    public DirectoryInfo? OutputPath { get; private set; }
    public int Watch { get; private set; }
    public bool Remove { get; private set; }
    public bool KillWhenAlive { get; private set; }
}

public static class DownloadOptions
{
    public sealed class Endpoint : Option<Uri>
    {
        public static Endpoint Instance { get; } = new();

        private Endpoint() : base("--endpoint", "The endpoint to download from")
        {
            AddAlias("-e");
            IsRequired = true;
            Arity = ArgumentArity.ExactlyOne;
        }
    }

    public sealed class Output : Option<DirectoryInfo>
    {
        public static Output Instance { get; } = new();

        private Output() : base("--output", "The directory to download to")
        {
            AddAlias("-o");
            IsRequired = true;
            Arity = ArgumentArity.ExactlyOne;
        }
    }

    public sealed class Watch : Option<int>
    {
        public static Watch Instance { get; } = new();

        private Watch() : base("--watch", "The interval to watch for new files")
        {
            AddAlias("-w");
            IsRequired = false;
            SetDefaultValue(60);
        }
    }

    public sealed class Remove : Option<bool>
    {
        public static Remove Instance { get; } = new();

        private Remove() : base("--remove", "Remove downloaded files after successful download")
        {
            AddAlias("--rm");
            SetDefaultValue(false);
            IsRequired = false;
        }
    }

    public sealed class KillWhenAlive : Option<bool>
    {
        public static KillWhenAlive Instance { get; } = new();

        private KillWhenAlive() : base("--killWhenAlive", "Kill the process when the server is still alive")
        {
            SetDefaultValue(false);
            IsRequired = false;
        }
    }
}