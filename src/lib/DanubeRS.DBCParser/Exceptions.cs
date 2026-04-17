namespace DanubeRS.DBCParser;

public class DbcParseException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public DbcParseException(IEnumerable<string> errors)
        : base($"DBC file has {errors.Count()} parse error(s)")
    {
        Errors = errors.ToList();
    }
}

public class SignalDecodeException : Exception
{
    public uint FrameId { get; }
    public string? SignalName { get; }

    public SignalDecodeException(uint frameId, string? signalName, string message)
        : base(message)
    {
        FrameId = frameId;
        SignalName = signalName;
    }
}
