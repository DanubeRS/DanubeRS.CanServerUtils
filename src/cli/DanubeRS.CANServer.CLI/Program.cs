using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DanubeRS.CANServer.CLI;
using DanubeRS.CANServer.CLI.Commands.Download;
using DanubeRS.CANServer.CLI.Configuration;
using DanubeRS.CANServer.CLI.Commands.Root;
using DanubeRS.CANServer.CLI.Commands.Upload;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await new CommandLineBuilder()
    .AddCommands()
    .UseHost(
        hostBuilder =>
        {
            hostBuilder.ConfigureServices(collection =>
                {
                    collection.AddCliServices();
                    
                    collection.AddScoped(provider => LoggerFactory.Create(builder =>
                    {
                        var context = provider.GetRequiredService<InvocationContext>();
                        var verbosity = context.ParseResult.GetValueForOption(RootOptions.Verbosity.Instance);
                        builder.AddSimpleConsole(options =>
                        {
                            options.UseUtcTimestamp = false;
                            options.TimestampFormat = "O";
                            options.IncludeScopes = true;
                        })
                            .SetMinimumLevel(LogLevel.Warning)
                            .AddFilter("DanubeRS", verbosity);
                    }));
                })
                .UseCommandHandler<DownloadCommand, DownloadCommandHandler>()
                .UseCommandHandler<UploadCommand, UploadCommandHandler>();
        })
    .Build()
    .InvokeAsync(args);
