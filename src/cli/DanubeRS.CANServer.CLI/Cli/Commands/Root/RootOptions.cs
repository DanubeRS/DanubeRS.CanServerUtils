using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Cli.Commands.Root;

public static class RootOptions
{
    public sealed class Verbosity : Option<LogLevel>
    {
        public static Verbosity Instance { get; } = new();

        private Verbosity() : base("verbosity",
            "Set the verbosity level that will be produced when running the tool")
        {
            AddAlias("v");
            SetDefaultValue(LogLevel.Information);
            Arity = ArgumentArity.ZeroOrOne;
        }
    }
}