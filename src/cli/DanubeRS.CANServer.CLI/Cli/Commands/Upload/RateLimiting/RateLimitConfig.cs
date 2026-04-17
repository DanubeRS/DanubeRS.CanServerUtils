using YamlDotNet.Serialization;

namespace DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

  public record RateLimitConfig
    {
        [YamlMember(Alias = "global")]
        public GlobalRule? Global { get; init; }

        [YamlMember(Alias = "frames")]
        public List<FrameRule>? Frames { get; init; }
    };

    public record GlobalRule
    {
        [YamlMember(Alias = "windowMs")]
        public int WindowMs { get; init; }

        [YamlMember(Alias = "aggregate")]
        public AggregateMode Aggregate { get; init; }
    };
