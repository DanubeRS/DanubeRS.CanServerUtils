using System.CommandLine;

namespace DanubeRS.CANServer.CLI.Commands.Download;

public class DownloadCommand : Command
{
    public static DownloadCommand Instance { get; } = new();

    private DownloadCommand() : base("download", "Download CAN log files from a CANServer device")
    {
        Aliases.Add("dl");
        Options.Add(DownloadOptions.Endpoint.Instance);
        Options.Add(DownloadOptions.Output.Instance);
        Options.Add(DownloadOptions.Watch.Instance);
        Options.Add(DownloadOptions.Remove.Instance);
        Options.Add(DownloadOptions.KillWhenAlive.Instance);
    }
}

public class DownloadOptionValues
{
    public static DownloadOptionValues Parse(ParseResult result)
    {
        return new DownloadOptionValues
        {
            Endpoint = result.GetValue(DownloadOptions.Endpoint.Instance)!,
            OutputPath = result.GetValue(DownloadOptions.Output.Instance),
            Watch = result.GetValue(DownloadOptions.Watch.Instance),
            Remove = result.GetValue(DownloadOptions.Remove.Instance),
            KillWhenAlive = result.GetValue(DownloadOptions.KillWhenAlive.Instance),
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

        private Endpoint() : base("--endpoint", "-e")
        {
            Description = "URL of the CANServer device to download from (e.g. http://192.168.4.1)";
            HelpName = "URL";
            Required = true;
            Arity = ArgumentArity.ExactlyOne;
            CustomParser = OptionParsers.UriParser;
        }
    }

    public sealed class Output : Option<DirectoryInfo>
    {
        public static Output Instance { get; } = new();

        private Output() : base("--output", "-o")
        {
            Description = "Local directory where downloaded log files will be saved";
            HelpName = "DIR";
            Required = true;
            Arity = ArgumentArity.ExactlyOne;
        }
    }

    public sealed class Watch : Option<int>
    {
        public static Watch Instance { get; } = new();

        private Watch() : base("--watch", "-w")
        {
            Description = "Poll interval in seconds to check for new files. Set to 0 for a single pass without polling";
            HelpName = "SECONDS";
            DefaultValueFactory = _ => 60;
        }
    }

    public sealed class Remove : Option<bool>
    {
        public static Remove Instance { get; } = new();

        private Remove() : base("--remove", "--rm")
        {
            Description = "Delete log files from the CANServer device after they have been successfully downloaded";
            DefaultValueFactory = _ => false;
        }
    }

    public sealed class KillWhenAlive : Option<bool>
    {
        public static KillWhenAlive Instance { get; } = new();

        private KillWhenAlive() : base("--kill")
        {
            Description = "Exit the process when the CANServer device is detected as active (bus traffic present). Useful for one-shot downloads before the device starts logging again";
            DefaultValueFactory = _ => false;
        }
    }
}
