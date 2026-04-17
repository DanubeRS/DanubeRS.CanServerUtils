using System.CommandLine;
using DanubeRS.CANServer.CLI;
using DanubeRS.CANServer.CLI.Commands.Download;
using DanubeRS.CANServer.CLI.Commands.Root;
using DanubeRS.CANServer.CLI.Commands.Upload;
using DanubeRS.CANServer.CLI.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var rootCommand = new RootCommand("CANServer CLI - Download and upload CAN log files");

var verbosityOption = RootOptions.Verbosity.Instance;
rootCommand.Options.Add(verbosityOption);

var parsedResult = rootCommand.Parse(args);
var verbosity = parsedResult.GetValue(verbosityOption);

var collection = new ServiceCollection();
collection.AddCliServices();

collection.AddScoped(_ => LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.UseUtcTimestamp = false;
        options.TimestampFormat = "O";
        options.IncludeScopes = true;
    })
        .SetMinimumLevel(LogLevel.Warning)
        .AddFilter("DanubeRS", verbosity);
}));

collection.AddScoped<DownloadCommandHandler>();
collection.AddScoped<UploadCommandHandler>();

await using var serviceProvider = collection.BuildServiceProvider();

var downloadCmd = DownloadCommand.Instance;
downloadCmd.SetScopedAction<DownloadCommandHandler>(serviceProvider);

var uploadCmd = UploadCommand.Instance;
uploadCmd.SetScopedAction<UploadCommandHandler>(serviceProvider);

rootCommand.Subcommands.Add(downloadCmd);
rootCommand.Subcommands.Add(uploadCmd);

return await parsedResult.InvokeAsync();