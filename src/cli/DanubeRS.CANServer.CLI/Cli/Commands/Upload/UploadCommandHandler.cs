using System.CommandLine;
using System.Diagnostics;
using DanubeRS.CANServer.Lib.BinaryLogs;
using DanubeRS.CanServerUtils.Lib;
using DanubeRS.CANServer.CLI.Configuration;
using DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Commands.Upload;

public class UploadCommandHandler(
    DatabaseFactory databaseFactory,
    ILogger<CanLogUploadPipeline> pipelineLogger,
    ILogger<UploadCommandHandler> logger) : ICommandAction
{
    public async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var uploadOptions = UploadOptionsValues.Parse(parseResult);

        var inputPath = parseResult.GetValue(UploadOptions.InputPath.Instance);
        var archiveDirectory = parseResult.GetValue(UploadOptions.ArchivePath.Instance);

        var watchInterval = parseResult.GetValue(UploadOptions.WatchInterval.Instance);
        var signals = parseResult.GetValue(UploadOptions.Signals.Instance);

        var influxClient = InfluxWriteClientFactory(uploadOptions.InfluxDb);
        var database = await databaseFactory(uploadOptions.Databases ?? []);
        while (!cancellationToken.IsCancellationRequested)
        {
            var inputDir = inputPath ?? new DirectoryInfo(Directory.GetCurrentDirectory());
            var files = inputDir.EnumerateFiles("*.log").OrderBy(f => f.Name);

            foreach (var file in files)
            {
                await ParseFileInternal(file, database, influxClient, uploadOptions);
                if (archiveDirectory != null)
                    file.MoveTo(Path.Combine(archiveDirectory.FullName, file.Name));
            }

            if (watchInterval <= 0) return 0;

            logger.LogInformation("Parsing is watching, delay {Seconds} before next loop...", watchInterval);
            await Task.Delay(TimeSpan.FromSeconds(watchInterval), cancellationToken);
        }

        return 0;
    }

    private WriteApiAsync InfluxWriteClientFactory(InfluxDbOptionsValues options)
    {
        var influxClient = new InfluxDBClient(options.Endpoint.ToString(), options.Token);
        if (!options.Compress)
            influxClient.DisableGzip();
        return influxClient.GetWriteApiAsync();
    }

    private async Task ParseFileInternal(FileInfo fileInfo,
        Database database, WriteApiAsync writeApiAsync, UploadOptionsValues uploadOptions)
    {
        var rateLimitConfig = await LoadRateLimitConfig(uploadOptions);

        var pipeline = new CanLogUploadPipeline(
            database,
            writeApiAsync,
            uploadOptions.InfluxDb.Bucket!,
            uploadOptions.InfluxDb.Org!,
            uploadOptions.Signals?.Any() == true ? uploadOptions.Signals : null,
            uploadOptions.BatchSize,
            pipelineLogger,
            rateLimitConfig);

        await pipeline.ProcessFilesAsync(new[] { fileInfo }, default);
    }

    private async Task<RateLimitConfig?> LoadRateLimitConfig(UploadOptionsValues options)
    {
        if (options.RateLimitConfig == null && !options.GlobalWindow.HasValue)
            return null;

        var loader = new RateLimitConfigLoader();
        
        if (options.RateLimitConfig != null)
        {
            logger.LogInformation("Loading rate limit config from {ConfigFile}", options.RateLimitConfig.FullName);
            
            var result = loader.Load(options.RateLimitConfig, options.GlobalWindow);
            
            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to load rate limit config: {result.Error?.Message}");

            return result.Value;
        }

       // Only --global-window specified without YAML file
        return new RateLimitConfig
        {
            Global = new GlobalRule { WindowMs = options.GlobalWindow!.Value, Aggregate = AggregateMode.Last },
            Frames = null
        };
    }
}
