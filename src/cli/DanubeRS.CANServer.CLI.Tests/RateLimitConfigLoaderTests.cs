using DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CANServer.CLI.Tests.RateLimiting;

public class RateLimitConfigLoaderTests : IDisposable
{
    private readonly string _tempDir;
    
    public RateLimitConfigLoaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_tempDir);
    }

    private FileInfo CreateTempFile(string yaml)
    {
        var path = Path.Combine(_tempDir, "config.yaml");
        File.WriteAllText(path, yaml);
        return new FileInfo(path);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    [Fact]
    public void Load_ValidYaml_ReturnsConfig()
    {
        var yaml = @"
global:
  windowMs: 100
  aggregate: mean
frames:
  - id: 512
    signals:
      - name: VehicleSpeed
        windowMs: 50
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        if (!result.IsSuccess)
        {
            var ex = result.Error;
            while (ex != null)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.WriteLine($"Stack: {ex.StackTrace?.Substring(0, Math.Min(200, ex.StackTrace.Length))}");
                ex = ex.InnerException;
            }
        }
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Global.WindowMs.Should().Be(100);
        result.Value.Global.Aggregate.Should().Be(AggregateMode.Mean);
        result.Value.Frames.Should().HaveCount(1);
    }

    [Fact]
    public void Load_MinimalYaml_UsesDefaults()
    {
        var yaml = @"
global:
  windowMs: 200
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Global.WindowMs.Should().Be(200);
        result.Value.Global.Aggregate.Should().Be(AggregateMode.Last);
    }

    [Fact]
    public void Load_NoGlobalSection_CreatesDefault()
    {
        var yaml = @"
frames:
  - id: 512
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Global.WindowMs.Should().Be(0);
        result.Value.Global.Aggregate.Should().Be(AggregateMode.Last);
    }

    [Fact]
    public void Load_NegativeWindowMsInGlobal_ReturnsError()
    {
        var yaml = @"
global:
  windowMs: -100
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Load_NegativeWindowMsInFrame_ReturnsError()
    {
        var yaml = @"
global:
  windowMs: 100
frames:
  - id: 512
    windowMs: -50
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Load_NegativeWindowMsInSignal_ReturnsError()
    {
        var yaml = @"
global:
  windowMs: 100
frames:
  - id: 512
    signals:
      - name: Speed
        windowMs: -30
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Load_CLIOverrideReplacesGlobalWindow()
    {
        var yaml = @"
global:
  windowMs: 100
  aggregate: mean
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), 500);

        result.IsSuccess.Should().BeTrue();
        result.Value.Global.WindowMs.Should().Be(500);
        result.Value.Global.Aggregate.Should().Be(AggregateMode.Mean);
    }

    [Fact]
    public void Load_CLIOverrideWithNoGlobal_CreatesNewGlobal()
    {
        var yaml = @"
frames:
  - id: 512
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), 250);

        result.IsSuccess.Should().BeTrue();
        result.Value.Global.WindowMs.Should().Be(250);
        result.Value.Global.Aggregate.Should().Be(AggregateMode.Last);
    }

    [Fact]
    public void Load_NegativeCLIWindow_ReturnsError()
    {
        var yaml = @"
global:
  windowMs: 100
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), -100);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Load_ComplexConfig_ParsesAllFields()
    {
        var yaml = @"
global:
  windowMs: 200
  aggregate: last
frames:
  - id: 512
    name: VehicleData
    windowMs: 150
    signals:
      - name: Speed
        windowMs: 100
        aggregate: mean
        ignore: false
      - name: RPM
        windowMs: 100
        aggregate: max
        ignore: true
  - id: 768
    signals:
      - name: BatteryTemp
";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Frames.Should().HaveCount(2);
        
        var frame512 = result.Value.Frames.First(f => f.Id == 512);
        frame512.Name.Should().Be("VehicleData");
        frame512.Signals.Should().HaveCount(2);
        
        var speedSignal = frame512.Signals!.First(s => s.Name == "Speed");
        speedSignal.Aggregate.Should().Be(AggregateMode.Mean);
        speedSignal.Ignore.Should().BeFalse();
    }

    [Fact]
    public void Load_InvalidYaml_ReturnsError()
    {
        var yaml = @"invalid: yaml: content: [";
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(CreateTempFile(yaml), null);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Load_FileNotFound_ReturnsError()
    {
        var loader = new RateLimitConfigLoader();

        var result = loader.Load(new FileInfo("/nonexistent/path/config.yaml"), null);

        result.IsSuccess.Should().BeFalse();
    }
}
