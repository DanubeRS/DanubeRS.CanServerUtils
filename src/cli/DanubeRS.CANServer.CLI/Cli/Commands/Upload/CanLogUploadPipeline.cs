using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using DanubeRS.CanServerUtils.Lib;
using DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;
using DanubeRS.CANServer.Lib.BinaryLogs;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Commands.Upload;

public sealed class CanLogUploadPipeline(
    Database database,
    WriteApiAsync writeApiAsync,
    string bucket,
    string org,
    uint[]? signalFilter,
    int batchSize,
    ILogger<CanLogUploadPipeline> logger,
    RateLimitConfig? rateLimitConfig = null)
{
    private readonly ConcurrentBag<DecodeError> _decodeErrors = new();
    private long _totalFramesProcessed;
    private long _totalPointsUploaded;
    private readonly SignalRateLimiter? _rateLimiter;

    internal IReadOnlyCollection<DecodeError> Errors => _decodeErrors;

    public CanLogUploadPipeline(
        Database database,
        WriteApiAsync writeApiAsync,
        string bucket,
        string org,
        uint[]? signalFilter,
        int batchSize,
        ILogger<CanLogUploadPipeline> logger)
        : this(database, writeApiAsync, bucket, org, signalFilter, batchSize, logger, null)
    {
    }

    public async Task ProcessFilesAsync(IEnumerable<FileInfo> files, CancellationToken cancellationToken)
    {
        var readerBlock = CreateReaderBlock();
        var decoderBlock = CreateDecoderBlock(database);
        var filterConverterBlock = CreateFilterConverterBlock(signalFilter);
        
        ISourceBlock<PointData>? rateLimiterSource;
        ITargetBlock<SignalData>? rateLimiterTarget;

        if (_rateLimiter != null)
        {
            var rateLimiterBlock = CreateRateLimiterBlock(_rateLimiter);
            rateLimiterSource = rateLimiterBlock;
            rateLimiterTarget = rateLimiterBlock;
        }
        else
        {
            var passthroughBlock = new TransformManyBlock<SignalData, PointData>(data => new[] { BuildPointFromData(data) },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount });
            rateLimiterSource = passthroughBlock;
            rateLimiterTarget = passthroughBlock;
        }

        var batcherBlock = new BatchBlock<PointData>(batchSize, 
            new GroupingDataflowBlockOptions { BoundedCapacity = 10 });
        var uploaderBlock = CreateUploaderBlock(writeApiAsync, bucket, org);

        readerBlock.LinkTo(decoderBlock, new DataflowLinkOptions { PropagateCompletion = true });
        decoderBlock.LinkTo(filterConverterBlock, new DataflowLinkOptions { PropagateCompletion = true });
        filterConverterBlock.LinkTo(rateLimiterTarget, new DataflowLinkOptions { PropagateCompletion = true });
        rateLimiterSource.LinkTo(batcherBlock, new DataflowLinkOptions { PropagateCompletion = true });
        batcherBlock.LinkTo(uploaderBlock, new DataflowLinkOptions { PropagateCompletion = true });

        var totalStopwatch = Stopwatch.StartNew();
        
        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested) break;

            await ProcessFileAsync(file, readerBlock, cancellationToken);
        }

        readerBlock.Complete();
        
        if (_rateLimiter != null)
            _rateLimiter.Flush();

        await uploaderBlock.Completion.WaitAsync(cancellationToken);

        totalStopwatch.Stop();
        
        logger.LogInformation("Processed {frames} frames in {elapsed}", 
            Interlocked.Read(ref _totalFramesProcessed), totalStopwatch.Elapsed.TotalSeconds);
        logger.LogInformation("Processed {points} points in {elapsed} ({pointsSec}/sec)", 
            Interlocked.Read(ref _totalPointsUploaded), totalStopwatch.Elapsed.TotalSeconds,
            (double)Interlocked.Read(ref _totalPointsUploaded) / totalStopwatch.Elapsed.TotalSeconds);

        if (_rateLimiter != null)
        {
            var processed = _rateLimiter.PointsProcessed;
            var emitted = _rateLimiter.PointsEmitted;
            var ignored = _rateLimiter.PointsIgnored;
            var windowsClosed = _rateLimiter.WindowsClosed;
            var reductionRatio = processed > 0 ? (1.0 - (double)emitted / processed) * 100 : 0;

            logger.LogInformation("Rate limiting: {processed} points processed, {emitted} emitted ({reduction:F1}% reduction), {ignored} ignored",
                processed, emitted, reductionRatio, ignored);
            
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("Windows closed: {count}, Avg values per window: {avgValuesPerWindow:F2}", 
                    windowsClosed, windowsClosed > 0 ? (double)emitted / windowsClosed : 0);
        }

        if (_decodeErrors.Count > 0)
        {
            logger.LogWarning("{ErrorCount} frames failed to decode", _decodeErrors.Count);
        }
    }

    private async Task ProcessFileAsync(FileInfo file, ITargetBlock<FrameWithContext> readerBlock, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reading file {LogFileName}", file.FullName);
        
        await using var fs = file.OpenRead();
        var reader = await Reader.Open(fs, logger);
        
        if (reader == null)
        {
            logger.LogWarning("Could not open file {LogFileName}", file.FullName);
            return;
        }

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var frame = reader.GetNextFrame();
            if (frame == null) break;

            await readerBlock.SendAsync(new FrameWithContext(frame, file.Name), cancellationToken);
        }
    }

    private TransformManyBlock<FrameWithContext, RawFrame> CreateReaderBlock()
    {
        return new TransformManyBlock<FrameWithContext, RawFrame>(
            ctx => new[] { new RawFrame(ctx.Frame.FrameTime, ctx.Frame.FrameId, ctx.Frame.BusId, ctx.Frame.FramePayload, ctx.SourceFileName) },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
                BoundedCapacity = 500
            });
    }

    private TransformBlock<RawFrame, DecodedMessage> CreateDecoderBlock(Database database)
    {
        long framesProcessed = 0;

        return new TransformBlock<RawFrame, DecodedMessage>(frame =>
        {
            logger.LogDebug("Decoding frame {FrameId:x8} @ {FrameTime}", frame.FrameId, frame.Timestamp);

            if (!database.TryParseBinaryMessage(frame.FrameId, frame.Payload, out var value, out var defn))
            {
                _decodeErrors.Add(new DecodeError(frame.FrameId, $"Unknown frame ID: 0x{frame.FrameId:X8}"));
                return null!;
            }

            Interlocked.Increment(ref framesProcessed);
            
            logger.LogDebug("Successfully Parsed message #{MessageNumber} @ {MessageTime} (ID:{FrameId})", 
                framesProcessed, frame.Timestamp, frame.FrameId);
            logger.LogTrace("{MessageId} {MessageName} {Signals}", value.MessageId, value.MessageName,
                string.Join(", ", value.Signals.Select(s => s.SignalName)));

            return new DecodedMessage(frame.Timestamp, frame.BusId, value, defn, frame.SourceFileName);
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            BoundedCapacity = 200,
            EnsureOrdered = true
        });
    }

    private TransformManyBlock<DecodedMessage, SignalData> CreateFilterConverterBlock(uint[]? signalFilter)
    {
        return new TransformManyBlock<DecodedMessage, SignalData>(msg =>
        {
            if (signalFilter?.Any() == true && !signalFilter.Contains(msg.Value.MessageId))
            {
                return Array.Empty<SignalData>();
            }

            var signals = new List<SignalData>();
            
            foreach (var signal in msg.Value.Signals)
            {
                var signalDfn = msg.Defn.Signals.First(s => s.Signal.Name == signal.SignalName);
                
                var tags = new Dictionary<string, string>
                {
                    ["bus"] = msg.BusId.ToString(),
                    ["frame_id"] = msg.Defn.Header.Id.ToString(),
                    ["frame_name"] = msg.Defn.Header.Name,
                    ["filename"] = msg.SourceFileName
                };

                signals.Add(new SignalData(
                    signal.SignalName,
                    signal.Value,
                    msg.Defn.Header.Id,
                    signalDfn.Signal.Unit.TrimStart('"').TrimEnd('"'),
                    tags,
                    DateTimeOffset.FromUnixTimeMilliseconds((long)msg.Timestamp)));
            }
            
            return signals;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            BoundedCapacity = DataflowBlockOptions.Unbounded
        });
    }

    private TransformManyBlock<SignalData, PointData> CreateRateLimiterBlock(SignalRateLimiter rateLimiter)
    {
        return new TransformManyBlock<SignalData, PointData>(data =>
        {
            var (point, consumed) = rateLimiter.TryProcessPoint(
                data.Value, data.SignalName, data.FrameId, data.Unit, data.Tags, data.Timestamp);

            if (consumed && point == null)
                return Array.Empty<PointData>();

            return new[] { point! };
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            BoundedCapacity = DataflowBlockOptions.Unbounded
        });
    }

    private PointData BuildPointFromData(SignalData data)
    {
        var point = PointData.Measurement("CAN")
            .Tag("_unit", data.Unit)
            .Field(data.SignalName, data.Value);

        foreach (var tag in data.Tags)
        {
            point.Tag(tag.Key, tag.Value);
        }

        return point.Timestamp(data.Timestamp.ToUnixTimeMilliseconds(), WritePrecision.Ms);
    }

    private ActionBlock<IEnumerable<PointData>> CreateUploaderBlock(WriteApiAsync writeApiAsync, string bucket, string org)
    {
        var pointsStopwatch = Stopwatch.StartNew();
        long pointsUploaded = 0;

        return new ActionBlock<IEnumerable<PointData>>(async batch =>
        {
            var rqStop = Stopwatch.StartNew();
            await writeApiAsync.WritePointsAsync(batch.ToList(), bucket, org);
            rqStop.Stop();

            var currentPoints = batch.Count();
            Interlocked.Add(ref pointsUploaded, currentPoints);

            logger.LogDebug("Processed {points} points ({pointsSec}/sec) (RQ: {RequestSec})", 
                pointsUploaded,
                pointsUploaded / pointsStopwatch.Elapsed.TotalSeconds, 
                rqStop.Elapsed.TotalSeconds);
            
            pointsStopwatch.Restart();
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 2,
            BoundedCapacity = DataflowBlockOptions.Unbounded
        });
    }

    internal record DecodeError(uint FrameId, string Message);
    
    private record RawFrame(ulong Timestamp, uint FrameId, uint BusId, byte[] Payload, string SourceFileName);
    
    private record DecodedMessage(ulong Timestamp, uint BusId, MessageValue Value, Database.MessageDefinition Defn, string SourceFileName);
    
    private record FrameWithContext(LogReaderFrame Frame, string SourceFileName);

    private record SignalData(
        string SignalName,
        double Value,
        uint FrameId,
        string Unit,
        Dictionary<string, string> Tags,
        DateTimeOffset Timestamp);
}
