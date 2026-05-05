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

                        var results = await pipeline.ProcessFilesAsync(files, cancellationToken);

                        if (uploadOptions.Archive != null)
                        {
                            ArchiveFiles(results, uploadOptions.Archive.Value, uploadOptions.ArchivePath ?? inputDir);
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

    private void ArchiveFiles(List<FileProcessingResult> results, ArchiveMode mode, DirectoryInfo archiveDirectory)
    {
        var timestampCounts = new Dictionary<string, int>();
        var dateFiles = new Dictionary<string, List<string>>();

        foreach (var result in results)
        {
            var timestamp = result.FirstTimestamp ?? result.File.CreationTime;
            var localTimestamp = timestamp.ToLocalTime();
            var timestampKey = localTimestamp.ToString("yyyy-MM-ddTHH-mm-ss");
            var dateKey = localTimestamp.ToString("yyyy-MM-dd");

            if (!timestampCounts.TryGetValue(timestampKey, out int increment))
            {
                increment = 0;
            }

            if (increment > 9)
            {
                logger.LogError("Cannot archive file {FileName}: more than 10 files share timestamp {TimestampKey}", result.File.Name, timestampKey);
                continue;
            }

            var archiveFileName = $"{timestampKey}_{increment}.log";
            var dateDir = Path.Combine(archiveDirectory.FullName, dateKey);
            Directory.CreateDirectory(dateDir);
            var archivePath = Path.Combine(dateDir, archiveFileName);

            try
            {
                result.File.MoveTo(archivePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to archive file {FileName} to {ArchivePath}", result.File.Name, archivePath);
                continue;
            }

            if (!dateFiles.ContainsKey(dateKey))
                dateFiles[dateKey] = new List<string>();
            dateFiles[dateKey].Add(archivePath);

            timestampCounts[timestampKey] = increment + 1;
        }

        if (mode == ArchiveMode.Tar)
        {
            CompressDateDirectories(dateFiles, archiveDirectory);
        }
    }

    private void CompressDateDirectories(Dictionary<string, List<string>> dateFiles, DirectoryInfo archiveDirectory)
    {
        foreach (var (dateKey, _) in dateFiles)
        {
            var dateDirPath = Path.Combine(archiveDirectory.FullName, dateKey);

            if (!Directory.Exists(dateDirPath))
                continue;

            ArchiveCompressor.CompressDateDirectory(dateKey, dateDirPath, archiveDirectory.FullName, logger);
        }
    }
}
