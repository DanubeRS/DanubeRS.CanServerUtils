using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DanubeRS.CANServer.CLI.Cli;
using DanubeRS.CANServer.CLI.Cli.Commands.Download;
using DanubeRS.CANServer.CLI.Cli.Commands.Root;
using DanubeRS.CANServer.CLI.Cli.Commands.Upload;
using DanubeRS.CANServer.Lib.BinaryLogs;
using DanubeRS.CANServer.Lib.Downloader;
using DanubeRS.CANServer.Lib.Downloader.Client;
using DanubeRS.DBCParser;
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
                    collection.AddHttpClient(nameof(Downloader));
                    collection.AddScoped<DownloaderFactory>(provider => uri =>
                    {
                        var httpClient = provider.GetRequiredService<IHttpClientFactory>()
                            .CreateClient(nameof(Downloader));
                        httpClient.BaseAddress = uri;
                        return new Downloader(new LogFileClient(httpClient),
                            provider.GetRequiredService<ILogger<Downloader>>());
                    });
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

                    collection.AddScoped<Database>();
                    collection.AddScoped<ReaderFactory>(provider =>
                        (stream => Reader.Open(stream, provider.GetRequiredService<ILogger<Reader>>())));
                    collection.AddScoped<DatabaseFactory>(provider => async paths =>
                    {
                        var database = provider.GetRequiredService<Database>();
                        foreach (var dbc in paths)
                        {
                            await using var dbcStream = dbc.OpenRead();
                            using var dbcReader = new StreamReader(dbcStream);
                            database.AddFile(dbcReader);
                        }

                        return database;
                    });
                })
                .UseCommandHandler<DownloadCommand, DownloadCommandHandler>()
                .UseCommandHandler<UploadCommand, UploadCommandHandler>();
        })
    .Build()
    .InvokeAsync(args);