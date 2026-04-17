using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

public sealed class RateLimitConfigLoader
{
    private readonly IDeserializer _deserializer;

    public RateLimitConfigLoader()
    {
        var config = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _deserializer = config;
    }

    public Result<RateLimitConfig> Load(FileInfo configFile, int? cliGlobalWindowOverride)
    {
        try
        {
            var yaml = File.ReadAllText(configFile.FullName);
            var config = _deserializer.Deserialize<RateLimitConfig>(yaml);

            if (config == null)
                return Result<RateLimitConfig>.Failure(new InvalidOperationException("YAML deserialization returned null"));

            // Apply defaults for missing aggregate modes (enum default is Mean, but we want Last)
            bool yamlSpecifiesAggregate = yaml.Contains("aggregate:");
            
            foreach (var frame in config.Frames ?? [])
            {
                if (frame.WindowMs < 0)
                    return Result<RateLimitConfig>.Failure(new ArgumentException($"Negative windowMs {frame.WindowMs} for frame {(frame.Id?.ToString() ?? frame.Name)}"));

                foreach (var signal in frame.Signals ?? [])
                {
                    if (signal.WindowMs < 0)
                        return Result<RateLimitConfig>.Failure(new ArgumentException($"Negative windowMs {signal.WindowMs} for signal {signal.Name}"));
                }
            }

            if (config.Global != null && config.Global.WindowMs < 0)
                return Result<RateLimitConfig>.Failure(new ArgumentException($"Negative global windowMs {config.Global.WindowMs}"));

            if (cliGlobalWindowOverride.HasValue)
            {
                if (cliGlobalWindowOverride < 0)
                    return Result<RateLimitConfig>.Failure(new ArgumentException($"Negative --global-window value {cliGlobalWindowOverride}"));

                config = new RateLimitConfig
                {
                    Global = new GlobalRule 
                    { 
                        WindowMs = cliGlobalWindowOverride.Value, 
                        Aggregate = yamlSpecifiesAggregate ? (config.Global?.Aggregate ?? AggregateMode.Last) : AggregateMode.Last
                    },
                    Frames = config.Frames
                };
            }
            else if (config.Global == null)
            {
                config = new RateLimitConfig
                {
                    Global = new GlobalRule { WindowMs = 0, Aggregate = AggregateMode.Last },
                    Frames = config.Frames
                };
            }
            else if (!yamlSpecifiesAggregate && config.Global.Aggregate == AggregateMode.Mean)
            {
                // Apply default of Last when aggregate not specified in YAML
                config = new RateLimitConfig
                {
                    Global = new GlobalRule 
                    { 
                        WindowMs = config.Global.WindowMs, 
                        Aggregate = AggregateMode.Last 
                    },
                    Frames = config.Frames
                };
            }

            return Result<RateLimitConfig>.Success(config);
        }
        catch (Exception ex) when (!(ex is ArgumentException or InvalidOperationException))
        {
            return Result<RateLimitConfig>.Failure(new Exception($"Failed to load rate limit config from {configFile.FullName}", ex));
        }
    }
}

public class Result<T>
{
    public static Result<T> Success(T value) => new(value: value);
    public static Result<T> Failure(Exception error) => new(error: error);

    private Result(T? value = default, Exception? error = null)
    {
        Value = value;
        Error = error;
        IsSuccess = error == null;
    }

    public T? Value { get; }
    public Exception? Error { get; }
    public bool IsSuccess { get; }
}
