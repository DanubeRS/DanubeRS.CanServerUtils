using DanubeRS.DBCParser;
using DanubeRS.CANServer.Lib.BinaryLogs;
using DanubeRS.CANServer.Lib.Downloader;
using DanubeRS.CANServer.Lib.Downloader.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Configuration;

public delegate Task<DanubeRS.DBCParser.Database> DatabaseFactory(FileInfo[] files);
public delegate Task<DanubeRS.CANServer.Lib.BinaryLogs.Reader?> ReaderFactory(Stream stream);

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCliServices(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(Downloader));
        
        services.AddScoped<DownloaderFactory>(provider => uri =>
        {
            var httpClient = provider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(nameof(Downloader));
            httpClient.BaseAddress = uri;
            return new Downloader(new LogFileClient(httpClient),
                provider.GetRequiredService<ILogger<Downloader>>());
        });

        services.AddScoped<Database>();
        
        services.AddScoped<ReaderFactory>(provider =>
            (stream => Reader.Open(stream, provider.GetRequiredService<ILogger<Reader>>())));
            
        services.AddScoped<DatabaseFactory>(provider => async paths =>
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

        return services;
    }
}
