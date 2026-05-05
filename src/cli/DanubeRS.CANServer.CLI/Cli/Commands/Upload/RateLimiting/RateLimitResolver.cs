namespace DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

public sealed class RateLimitResolver(RateLimitConfig config)
{
    private readonly RateLimitConfig _config = config;
    private readonly Dictionary<uint, FrameRule> _frameRules = BuildFrameRules(config.Frames);

    private static Dictionary<uint, FrameRule> BuildFrameRules(List<FrameRule>? frames)
    {
        var dict = new Dictionary<uint, FrameRule>();
        if (frames == null) return dict;

        foreach (var frame in frames)
        {
            if (frame.Id.HasValue)
                dict.TryAdd((uint)frame.Id.Value, frame);
            else if (uint.TryParse(frame.Name, out var nameAsInt))
                dict.TryAdd(nameAsInt, frame);
        }

        return dict;
    }

    public SignalRule? GetRule(uint frameId, string signalName)
    {
        if (_frameRules.TryGetValue(frameId, out var matchingFrame))
        {
            var signalRule = matchingFrame.Signals?.FirstOrDefault(s => s.Name == signalName);
            
            if (signalRule != null)
                return signalRule;

            return new SignalRule 
            { 
                Name = signalName,
                WindowMs = matchingFrame.WindowMs,
                Aggregate = matchingFrame.Aggregate,
                Ignore = matchingFrame.Ignore
            };
        }

        return _config.Global != null 
            ? new SignalRule 
            { 
                Name = signalName, 
                WindowMs = _config.Global.WindowMs, 
                Aggregate = _config.Global.Aggregate,
                Ignore = false 
            }
            : null;
    }
}