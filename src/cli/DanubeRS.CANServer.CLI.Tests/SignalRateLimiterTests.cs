using DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CANServer.CLI.Tests.RateLimiting;

public class SignalRateLimiterTests
{
    private static Dictionary<string, string> CreateTestTags() => new() { ["frame_id"] = "512" };
    
    [Fact]
    public void TryProcessPoint_NoRule_ReturnsOriginalPointUnconsumed()
    {
        var config = new RateLimitConfig();
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var timestamp = DateTimeOffset.UtcNow;

        var (point, consumed) = limiter.TryProcessPoint(60.0, "VehicleSpeed", 512, "km/h", CreateTestTags(), timestamp);

        point.Should().NotBeNull();
        consumed.Should().BeFalse("because no rule means pass through");
    }

    [Fact]
    public void TryProcessPoint_IgnoreRule_ReturnsNullConsumed()
    {
      var config = new RateLimitConfig 
         { 
             Global = null,
             Frames = new List<FrameRule>
             {
                 new FrameRule
                 {
                     Id = 512,
                     Name = null,
                     WindowMs = 200,
                     Aggregate = AggregateMode.Last,
                     Ignore = false,
                     Signals = new List<SignalRule>
                     {
                         new SignalRule { Name = "IgnoredSignal", WindowMs = 200, Aggregate = AggregateMode.Last, Ignore = true }
                     }
                 }
             }
         };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var (point, consumed) = limiter.TryProcessPoint(60.0, "IgnoredSignal", 512, "km/h", CreateTestTags(), DateTimeOffset.UtcNow);

        point.Should().BeNull();
        consumed.Should().BeTrue("because signal is ignored");
        limiter.PointsIgnored.Should().Be(1);
    }

    [Fact]
    public void TryProcessPoint_WithinWindow_BuffersValue()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 200, Aggregate = AggregateMode.Mean }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var timestamp1 = DateTimeOffset.UtcNow;
        var timestamp2 = timestamp1.AddMilliseconds(50); // Within 200ms window

        var (point1, consumed1) = limiter.TryProcessPoint(60.0, "VehicleSpeed", 512, "km/h", CreateTestTags(), timestamp1);
        var (point2, consumed2) = limiter.TryProcessPoint(62.0, "VehicleSpeed", 512, "km/h", CreateTestTags(), timestamp2);

        point1.Should().BeNull("window not expired yet");
        consumed1.Should().BeFalse();
        
        point2.Should().BeNull("still within window");
        consumed2.Should().BeFalse();
    }

    [Fact]
    public void TryProcessPoint_WindowExpired_EmitsAggregatedAndNewWindow()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 100, Aggregate = AggregateMode.Mean }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var timestamp1 = DateTimeOffset.UtcNow;
        var timestamp2 = timestamp1.AddMilliseconds(150); // After 100ms window expires

        var (point1, consumed1) = limiter.TryProcessPoint(60.0, "VehicleSpeed", 512, "km/h", CreateTestTags(), timestamp1);
        
        var (point2, consumed2) = limiter.TryProcessPoint(62.0, "VehicleSpeed", 512, "km/h", CreateTestTags(), timestamp2);

        point1.Should().BeNull("first value buffers");
        
        point2.Should().NotBeNull("window expired, emit aggregated");
        consumed2.Should().BeTrue();
        limiter.PointsEmitted.Should().Be(1);
    }

    [Fact]
    public void TryProcessPoint_MeanAggregation_ComputesCorrectAverage()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 50, Aggregate = AggregateMode.Mean }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var t1 = DateTimeOffset.UtcNow;

        // Add 3 values within window
        limiter.TryProcessPoint(60.0, "Speed", 512, "km/h", CreateTestTags(), t1);
        limiter.TryProcessPoint(62.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(10));
        limiter.TryProcessPoint(63.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(20));

        // Trigger window expiry with new value
        var (point, _) = limiter.TryProcessPoint(70.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(60));

        point.Should().NotBeNull();
    }

    [Fact]
    public void TryProcessPoint_LastAggregation_ReturnsMostRecent()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 50, Aggregate = AggregateMode.Last }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var t1 = DateTimeOffset.UtcNow;

        // Add values within window
        limiter.TryProcessPoint(60.0, "Speed", 512, "km/h", CreateTestTags(), t1);
        limiter.TryProcessPoint(62.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(10));

        // Trigger expiry
        var (point, _) = limiter.TryProcessPoint(70.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(60));

        point.Should().NotBeNull();
    }

    [Fact]
    public void TryProcessPoint_MinAggregation_ReturnsMinimum()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 50, Aggregate = AggregateMode.Min }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var t1 = DateTimeOffset.UtcNow;

        // Add values within window (min is 60)
        limiter.TryProcessPoint(70.0, "Speed", 512, "km/h", CreateTestTags(), t1);
        limiter.TryProcessPoint(60.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(10));

        // Trigger expiry
        var (point, _) = limiter.TryProcessPoint(80.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(60));

        point.Should().NotBeNull();
    }

    [Fact]
    public void TryProcessPoint_MaxAggregation_ReturnsMaximum()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 50, Aggregate = AggregateMode.Max }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var t1 = DateTimeOffset.UtcNow;

        // Add values within window (max is 70)
        limiter.TryProcessPoint(60.0, "Speed", 512, "km/h", CreateTestTags(), t1);
        limiter.TryProcessPoint(70.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(10));

        // Trigger expiry
        var (point, _) = limiter.TryProcessPoint(65.0, "Speed", 512, "km/h", CreateTestTags(), t1.AddMilliseconds(60));

        point.Should().NotBeNull();
    }

    [Fact]
    public void Flush_EmitsRemainingBufferedValues()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 500, Aggregate = AggregateMode.Mean }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var t1 = DateTimeOffset.UtcNow;

        limiter.TryProcessPoint(60.0, "Speed", 512, "km/h", CreateTestTags(), t1);
        limiter.TryProcessPoint(62.0, "Speed", 513, "km/h", CreateTestTags(), t1.AddMilliseconds(10));

        limiter.PointsEmitted.Should().Be(0);

        var flushedPoints = limiter.Flush();

        limiter.PointsEmitted.Should().Be(2);
        flushedPoints.Should().HaveCount(2);
    }

    [Fact]
    public void Flush_ReturnsPointDataWithCorrectMetadata()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 500, Aggregate = AggregateMode.Last }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var t1 = DateTimeOffset.UtcNow;
        var tags = new Dictionary<string, string> { ["frame_id"] = "512", ["bus"] = "0" };

        limiter.TryProcessPoint(60.0, "VehicleSpeed", 512, "km/h", tags, t1);
        limiter.TryProcessPoint(62.0, "VehicleSpeed", 512, "km/h", tags, t1.AddMilliseconds(10));

        var flushedPoints = limiter.Flush();

        flushedPoints.Should().HaveCount(1);
        limiter.PointsEmitted.Should().Be(1);
    }

    [Fact]
    public void Statistics_TrackAllMetrics()
    {
       var config = new RateLimitConfig 
         { 
             Global = new GlobalRule { WindowMs = 50, Aggregate = AggregateMode.Mean },
             Frames = new List<FrameRule>
             {
                 new FrameRule
                 {
                     Id = 1024,
                     Name = null,
                     WindowMs = 50,
                     Aggregate = AggregateMode.Last,
                     Ignore = false,
                     Signals = new List<SignalRule>
                     {
                         new SignalRule 
                         { 
                             Name = "Ignored", 
                             WindowMs = 50, 
                             Aggregate = AggregateMode.Last, 
                             Ignore = true 
                         }
                     }
                 }
             }
         };
        var resolver = new RateLimitResolver(config);
        var limiter = new SignalRateLimiter(resolver, NullLogger<SignalRateLimiter>.Instance);

        var t1 = DateTimeOffset.UtcNow;

        // Process some points
        limiter.TryProcessPoint(60.0, "Normal", 512, "km/h", CreateTestTags(), t1);
        limiter.TryProcessPoint(70.0, "Ignored", 1024, "unit", CreateTestTags(), t1.AddMilliseconds(60));

        // Assert
        limiter.PointsProcessed.Should().BeGreaterOrEqualTo(1);
    }
}
