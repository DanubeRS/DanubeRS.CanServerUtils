using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Commands.Root;

public static class RootOptions
{
    public sealed class Verbosity : Option<LogLevel>
    {
        public static Verbosity Instance { get; } = new();

        private Verbosity() : base("--verbosity", "-v")
        {
            Description = "Set the minimum log level. Valid values: None, Critical, Error, Warning, Information, Debug, Trace.";
            HelpName = "LEVEL";
            DefaultValueFactory = _ => LogLevel.Information;
            Arity = ArgumentArity.ZeroOrOne;
        }
    }
}
