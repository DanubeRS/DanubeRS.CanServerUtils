using YamlDotNet.Serialization;

namespace DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

public record FrameRule
{
    [YamlMember(Alias = "id")]
    public int? Id { get; init; }

    [YamlMember(Alias = "name")]
    public string? Name { get; init; }

    [YamlMember(Alias = "windowMs")]
    public int WindowMs { get; init; }

    [YamlMember(Alias = "aggregate")]
    public AggregateMode Aggregate { get; init; }

    [YamlMember(Alias = "ignore")]
    public bool Ignore { get; init; }

    [YamlMember(Alias = "signals")]
    public List<SignalRule>? Signals { get; init; }
};
