using YamlDotNet.Serialization;

namespace DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

public record SignalRule
{
    [YamlMember(Alias = "name")]
    public string Name { get; init; } = string.Empty;

    [YamlMember(Alias = "windowMs")]
    public int WindowMs { get; init; }

    [YamlMember(Alias = "aggregate")]
    public AggregateMode Aggregate { get; init; }

    [YamlMember(Alias = "ignore")]
    public bool Ignore { get; init; }
};
