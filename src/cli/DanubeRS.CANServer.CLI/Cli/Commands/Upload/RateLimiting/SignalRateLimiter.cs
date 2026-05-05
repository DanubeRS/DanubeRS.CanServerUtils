using System.Collections.Concurrent;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

public sealed class SignalRateLimiter(RateLimitResolver resolver, ILogger<SignalRateLimiter>? logger = null)
{
    private readonly ConcurrentDictionary<string, SignalWindowState> _state = new();
    private readonly RateLimitResolver _resolver = resolver;
    private readonly ILogger<SignalRateLimiter>? _logger = logger;

    private long _pointsProcessed;
    private long _pointsEmitted;
    private long _pointsIgnored;
    private long _windowsClosed;

    public long PointsProcessed => Volatile.Read(ref _pointsProcessed);
    public long PointsEmitted => Volatile.Read(ref _pointsEmitted);
    public long PointsIgnored => Volatile.Read(ref _pointsIgnored);
    public long WindowsClosed => Volatile.Read(ref _windowsClosed);

    public (PointData? point, bool consumed) TryProcessPoint(
        double value, 
        string signalName, 
        uint frameId,
        string unit,
        Dictionary<string, string> tags,
        DateTimeOffset timestamp)
    {
        Interlocked.Increment(ref _pointsProcessed);

        var rule = _resolver.GetRule(frameId, signalName);
        
        if (rule == null)
            return (BuildPoint(signalName, value, unit, tags, timestamp), false);

        if (rule.Ignore)
        {
            Interlocked.Increment(ref _pointsIgnored);
            return (null, true);
        }

        var key = $"{frameId}:{signalName}";
        var timestampMs = timestamp.ToUnixTimeMilliseconds();

        var existingState = _state.GetOrAdd(key, _ => new SignalWindowState(
            timestampMs,
            rule.WindowMs,
            rule.Aggregate,
            signalName,
            unit,
            tags,
            timestampMs));

        if (timestampMs - existingState.WindowStart >= rule.WindowMs)
        {
            var emittedValue = AggregateAndEmit(existingState, key);
            Interlocked.Increment(ref _pointsEmitted);
            Interlocked.Increment(ref _windowsClosed);

            _state[key] = new SignalWindowState(
                timestampMs,
                rule.WindowMs,
                rule.Aggregate,
                signalName,
                unit,
                tags,
                timestampMs);

            return (BuildPoint(signalName, emittedValue, unit, tags, timestamp), true);
        }
        else
        {
            existingState.Values.Add(value);
            return (null, false);
        }
    }

    private PointData BuildPoint(string signalName, double value, string unit, Dictionary<string, string> tags, DateTimeOffset timestamp)
    {
        var point = PointData.Measurement("CAN")
            .Tag("_unit", unit)
            .Field(signalName, value);

        foreach (var tag in tags.Where(t => t.Key != "_unit"))
        {
            point.Tag(tag.Key, tag.Value);
        }

        return point.Timestamp(timestamp.ToUnixTimeMilliseconds(), WritePrecision.Ms);
    }

    private double AggregateAndEmit(SignalWindowState state, string key)
    {
        var aggregatedValue = ComputeAggregate(state.Values, state.AggregateMode);

        if (_logger?.IsEnabled(LogLevel.Trace) == true)
            _logger.LogTrace("Aggregated {Count} values for window {Key}: [{Values}] -> {Aggregate}", 
                state.Values.Count, key, string.Join(", ", state.Values), aggregatedValue);

        return aggregatedValue;
    }

    private static double ComputeAggregate(List<double> values, AggregateMode mode)
    {
        if (values.Count == 0)
            return 0;

        return mode switch
        {
            AggregateMode.Mean => values.Average(),
            AggregateMode.Median => GetMedian(values),
            AggregateMode.Min => values.Min(),
            AggregateMode.Max => values.Max(),
            AggregateMode.Last or _ => values[^1]
        };
    }

    private static double GetMedian(List<double> values)
    {
        var count = values.Count;
        if (count == 0) return 0;
        
        var sorted = new List<double>(values);
        sorted.Sort();
        
        if (count % 2 == 1)
            return sorted[count / 2];
            
        var midIndex = count / 2 - 1;
        return (sorted[midIndex] + sorted[midIndex + 1]) / 2.0;
    }

   public List<PointData> Flush()
    {
        var flushedPoints = new List<PointData>();

        foreach (var kvp in _state)
        {
            if (kvp.Value.Values.Count > 0)
            {
                var aggregatedValue = ComputeAggregate(kvp.Value.Values, kvp.Value.AggregateMode);

                if (kvp.Value.SignalName != null && kvp.Value.Tags != null)
                {
                    Interlocked.Increment(ref _pointsEmitted);
                    Interlocked.Increment(ref _windowsClosed);

                    var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(kvp.Value.LastTimestampMs);
                    var point = BuildPoint(kvp.Value.SignalName, aggregatedValue, kvp.Value.Unit ?? "", kvp.Value.Tags, timestamp);
                    flushedPoints.Add(point);
                }

                if (_logger?.IsEnabled(LogLevel.Debug) == true)
                    _logger.LogDebug("Flushed remaining window {Key} with {Count} values -> {Agg}", 
                        kvp.Key, kvp.Value.Values.Count, aggregatedValue);
            }
        }

        return flushedPoints;
    }
}

public record SignalWindowState(
    long WindowStart,
    int WindowMs,
    AggregateMode AggregateMode,
    string? SignalName = null,
    string? Unit = null,
    Dictionary<string, string>? Tags = null,
    long LastTimestampMs = 0)
{
    public List<double> Values { get; } = new();
}
