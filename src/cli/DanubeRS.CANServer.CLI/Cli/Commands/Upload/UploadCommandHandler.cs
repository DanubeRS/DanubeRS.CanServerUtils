using System.CommandLine.Invocation;
using System.Diagnostics;
using DanubeRS.CANServer.Lib.BinaryLogs;
using DanubeRS.CanServerUtils.Lib;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Cli.Commands.Upload;

public delegate Task<Database> DatabaseFactory(FileInfo[] files);

public delegate Task<Reader?> ReaderFactory(Stream stream);

public class UploadCommandHandler(
    DatabaseFactory databaseFactory,
    ReaderFactory readerFactory,
    ILogger<UploadCommandHandler> logger) : CommandHandlerBase
{
    protected override async Task<int> Run(InvocationContext context, CancellationToken cancellationToken)
    {
        var uploadOptions = UploadOptionsValues.Parse(context.ParseResult);

        var inputPath = context.ParseResult.CommandResult.GetValueForOption(UploadOptions.InputPath.Instance);
        var archiveDirectory = context.ParseResult.CommandResult.GetValueForOption(UploadOptions.ArchivePath.Instance);

        var watchInterval = context.ParseResult.CommandResult.GetValueForOption(UploadOptions.WatchInterval.Instance);
        var signals = context.ParseResult.CommandResult.GetValueForOption(UploadOptions.Signals.Instance);

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
        var stopWatch = new Stopwatch();
        var pointData = new List<PointData>();
        logger.LogInformation("Reading file {LogFileName}", fileInfo.FullName);
        await using var fs = fileInfo.OpenRead();
        var reader = await readerFactory(fs);
        if (reader == null)
        {
            logger.LogWarning("Could not open file {LogFileName}", fileInfo.FullName);
            return;
        }

        var frames = 0L;
        var points = 0L;
        stopWatch.Restart();
        var initialFrameTimeLogged = false;
        var pointsStopwatch = Stopwatch.StartNew();
        while (true)
        {
            var frame = reader.GetNextFrame();
            if (frame == null)
                break;
            logger.LogDebug("Decoding frame {FrameId:x8} @ {FrameTime}", frame.FrameId, frame.FrameTime);
            if (database.TryParseBinaryMessage(frame.FrameId, frame.FramePayload, out var value, out var defn))
            {
                if ((uploadOptions.Signals?.Any() ?? false) && !uploadOptions.Signals.Contains(frame.FrameId)) continue;
                logger.LogDebug("Successfully Parsed message #{MessageNumber} @ {MessageTime} (ID:{FrameId})", frames,
                    frame.FrameTime, frame.FrameId);
                logger.LogTrace("{MessageId} {MessageName} {Signals}", value.MessageId, value.MessageName,
                    value.Signals.Select(s => $"{s.SignalName}"));

                pointData.AddRange(value.Signals
                    .Select(signal => new
                        { signal, signalDfn = defn.Signals.First(s => s.Signal.Name == signal.SignalName) })
                    .Select(t => PointData.Measurement("CAN")
                        .Tag("_unit", t.signalDfn.Signal.Unit.TrimStart('"').TrimEnd('"'))
                        .Field(t.signal.SignalName, t.signal.Value)
                        .Tag("bus", frame.BusId.ToString())
                        .Tag("frame_id", defn.Header.Id.ToString())
                        .Tag("frame_name", defn.Header.Name)
                        .Timestamp((long)frame.FrameTime / 1000, WritePrecision.Ms)));
            }

            if (pointData.Count > 5_000)
            {
                var rqStop = Stopwatch.StartNew();
                await writeApiAsync.WritePointsAsync(pointData, uploadOptions.InfluxDb.Bucket,
                    uploadOptions.InfluxDb.Org);
                rqStop.Stop();
                logger.LogDebug("Processed {points} points ({pointsSec}/sec) (RQ: {RequestSec})", pointData.Count,
                    (pointData.Count / pointsStopwatch.Elapsed.TotalSeconds), rqStop.Elapsed.TotalSeconds);
                pointsStopwatch.Restart();
                points += pointData.Count;
                pointData = new List<PointData>();
            }

            if (!initialFrameTimeLogged)
            {
                logger.LogInformation("First file frame time {Timestamp:s}",
                    DateTime.UnixEpoch.AddMicroseconds(frame.FrameTime));
                initialFrameTimeLogged = true;
            }

            frames++;
            if (frames % 10_000 == 0)
            {
                logger.LogDebug("Current frame time {Timestamp:s}",
                    DateTime.UnixEpoch.AddMicroseconds(frame.FrameTime));
                logger.LogDebug("Processed {frames} frames ({framesSec}/sec)", frames,
                    (frames / stopWatch.Elapsed.TotalSeconds));
            }
        }

        points += pointData.Count;
        await writeApiAsync.WritePointsAsync(pointData, uploadOptions.InfluxDb.Bucket, uploadOptions.InfluxDb.Org);
        logger.LogDebug("Processed {points} points ({pointsSec}/sec)", points,
            (points / stopWatch.Elapsed.TotalSeconds));

        stopWatch.Stop();
        logger.LogInformation("Processed {frames} frames from {path} in {elapsed} ({framesSec}/sec)", frames,
            fileInfo.Name,
            stopWatch.Elapsed.TotalSeconds, frames / stopWatch.Elapsed.TotalSeconds);
        logger.LogInformation("Processed {points} points in {elapsed}({pointsSec}/sec)", points,
            stopWatch.Elapsed.TotalSeconds,
            (points / stopWatch.Elapsed.TotalSeconds));
    }
}