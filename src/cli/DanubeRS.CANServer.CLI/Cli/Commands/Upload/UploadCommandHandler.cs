using System.CommandLine;
using DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;
using DanubeRS.CANServer.CLI.Configuration;
using InfluxDB.Client;
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

        var influxDbOptions = uploadOptions.InfluxDb;
        var influxClient = new InfluxDBClient(influxDbOptions.Endpoint.ToString(), influxDbOptions.Token);
        if (!influxDbOptions.Compress)
            influxClient.DisableGzip();
        try
        {
            var writeApi = influxClient.GetWriteApiAsync();
            var database = await databaseFactory(uploadOptions.Databases ?? []);
            var rateLimitConfig = await LoadRateLimitConfig(uploadOptions);

            while (!cancellationToken.IsCancellationRequested)
            {
                var inputDir = inputPath ?? new DirectoryInfo(Directory.GetCurrentDirectory());
                var files = inputDir.EnumerateFiles("*.log").OrderBy(f => f.Name).ToArray();

                if (files.Length > 0)
                {
                    try
                    {
                        var pipeline = new CanLogUploadPipeline(
                            database,
                            writeApi,
                            uploadOptions.InfluxDb.Bucket!,
                            uploadOptions.InfluxDb.Org!,
                            uploadOptions.Signals?.Any() == true ? uploadOptions.Signals : null,
                            uploadOptions.BatchSize,
                            pipelineLogger,
                            rateLimitConfig);

                        await pipeline.ProcessFilesAsync(files, cancellationToken);

                        if (archiveDirectory != null)
                        {
                            foreach (var file in files)
                            {
                                try
                                {
                                    file.MoveTo(Path.Combine(archiveDirectory.FullName, file.Name));
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "Failed to archive file {FileName}", file.Name);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to process file batch");
                    }
                }

                if (watchInterval <= 0) return 0;

                logger.LogInformation("Parsing is watching, delay {Seconds} before next loop...", watchInterval);
                await Task.Delay(TimeSpan.FromSeconds(watchInterval), cancellationToken);
            }

            return 0;
        }
        finally
        {
            influxClient.Dispose();
        }
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

        return new RateLimitConfig
        {
            Global = new GlobalRule { WindowMs = options.GlobalWindow!.Value, Aggregate = AggregateMode.Last },
            Frames = null
        };
    }
}
