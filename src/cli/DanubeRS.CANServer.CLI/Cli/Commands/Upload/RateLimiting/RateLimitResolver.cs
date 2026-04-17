namespace DanubeRS.CANServer.CLI.Commands.Upload.RateLimiting;

public sealed class RateLimitResolver(RateLimitConfig config)
{
    private readonly RateLimitConfig _config = config;

    public SignalRule? GetRule(uint frameId, string signalName)
    {
        var matchingFrame = _config.Frames?.FirstOrDefault(f => 
            (f.Id.HasValue && f.Id.Value == frameId) ||
            (uint.TryParse(f.Name, out var nameAsInt) && nameAsInt == frameId));

        if (matchingFrame != null)
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
