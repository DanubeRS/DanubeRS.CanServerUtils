using DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

namespace DanubeRS.CANServer.CLI.Tests.RateLimiting;

public class RateLimitResolverTests
{
    [Fact]
    public void GetRule_NoConfig_ReturnsNull()
    {
        var config = new RateLimitConfig();
        var resolver = new RateLimitResolver(config);

        var result = resolver.GetRule(512, "VehicleSpeed");

        result.Should().BeNull();
    }

    [Fact]
    public void GetRule_GlobalOnly_ReturnsGlobalDefaults()
    {
        var config = new RateLimitConfig 
        { 
            Global = new GlobalRule { WindowMs = 200, Aggregate = AggregateMode.Mean }, 
            Frames = null 
        };
        var resolver = new RateLimitResolver(config);

        var result = resolver.GetRule(512, "VehicleSpeed");

        result.Should().NotBeNull();
        result!.Name.Should().Be("VehicleSpeed");
        result.WindowMs.Should().Be(200);
        result.Aggregate.Should().Be(AggregateMode.Mean);
        result.Ignore.Should().BeFalse();
    }

    [Fact]
    public void GetRule_FrameIdMatch_ReturnsFrameDefaults()
    {
          var config = new RateLimitConfig 
         { 
             Global = new GlobalRule { WindowMs = 200, Aggregate = AggregateMode.Last },
             Frames = new List<FrameRule>
             {
                 new FrameRule { Id = 512, Name = null, WindowMs = 300, Aggregate = AggregateMode.Mean, Ignore = false, Signals = null }
             }
         };
        var resolver = new RateLimitResolver(config);

        var result = resolver.GetRule(512, "VehicleSpeed");

        result.Should().NotBeNull();
        result!.WindowMs.Should().Be(300);
        result.Aggregate.Should().Be(AggregateMode.Mean);
    }

    [Fact]
    public void GetRule_FrameNameMatch_ReturnsFrameDefaults()
    {
         var config = new RateLimitConfig 
         { 
             Global = new GlobalRule { WindowMs = 200, Aggregate = AggregateMode.Last },
             Frames = new List<FrameRule>
             {
                 new FrameRule { Id = null, Name = "512", WindowMs = 300, Aggregate = AggregateMode.Max, Ignore = false, Signals = null }
             }
         };
        var resolver = new RateLimitResolver(config);

        var result = resolver.GetRule(512, "VehicleSpeed");

        result.Should().NotBeNull();
        result!.WindowMs.Should().Be(300);
        result.Aggregate.Should().Be(AggregateMode.Max);
    }

    [Fact]
    public void GetRule_SignalOverride_ReturnsSignalSpecific()
    {
      var config = new RateLimitConfig 
         { 
             Global = new GlobalRule { WindowMs = 200, Aggregate = AggregateMode.Last },
             Frames = new List<FrameRule>
             {
                 new FrameRule 
                 {
                     Id = 512,
                     Name = null,
                     WindowMs = 300,
                     Aggregate = AggregateMode.Mean,
                     Ignore = false,
                     Signals = new List<SignalRule>
                     {
                         new SignalRule { Name = "VehicleSpeed", WindowMs = 500, Aggregate = AggregateMode.Min, Ignore = false }
                     }
                 }
             }
         };
        var resolver = new RateLimitResolver(config);

        var result = resolver.GetRule(512, "VehicleSpeed");

        result.Should().NotBeNull();
        result!.WindowMs.Should().Be(500);
        result.Aggregate.Should().Be(AggregateMode.Min);
    }

    [Fact]
    public void GetRule_SignalIgnore_ReturnsIgnored()
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
                        new SignalRule 
                        { 
                            Name = "NoisySensor", 
                            WindowMs = 200, 
                            Aggregate = AggregateMode.Last, 
                            Ignore = true 
                        }
                    }
                }
            }
        };
        var resolver = new RateLimitResolver(config);

        var result = resolver.GetRule(512, "NoisySensor");

        result.Should().NotBeNull();
        result!.Ignore.Should().BeTrue();
    }
}
