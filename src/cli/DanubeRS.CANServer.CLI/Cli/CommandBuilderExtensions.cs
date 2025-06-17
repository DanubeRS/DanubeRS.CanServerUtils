using System.CommandLine;
using System.CommandLine.Builder;
using DanubeRS.CANServer.CLI.Cli.Commands.Download;
using DanubeRS.CANServer.CLI.Cli.Commands.Root;
using DanubeRS.CANServer.CLI.Cli.Commands.Upload;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

namespace DanubeRS.CANServer.CLI.Cli;

public static class CommandBuilderExtensions
{
    public static CommandLineBuilder AddCommands(this CommandLineBuilder builder)
    {
        return builder.AddRootOptions().AddUploadCommand().AddDownloadCommand().UseHelp();
    }

    private static CommandLineBuilder AddRootOptions(this CommandLineBuilder builder)
    {
        var root = builder.Command;
        var verbosityOption = new Option<LogLevel>("verbosity", () => LogLevel.Information, "Set the verbosity level");
        verbosityOption.AddAlias("v");
        root.AddGlobalOption(verbosityOption);
        return builder;
    }

    private static CommandLineBuilder AddDownloadCommand(this CommandLineBuilder builder)
    {
        builder.Command.AddCommand(DownloadCommand.Instance);
        return builder;
    }

    private static CommandLineBuilder AddUploadCommand(this CommandLineBuilder builder)
    {
        builder.Command.AddCommand(UploadCommand.Instance);
        return builder;
    }
}