namespace DanubeRS.DBCParser;

/// <summary>
/// Positioned DBC failure: derives from FormatException so existing catch sites keep working,
/// while Errors carries one message per failure with position and context.
/// </summary>
public class DbcParseException : FormatException
{
    public IReadOnlyCollection<string> Errors { get; }

    public DbcParseException(IEnumerable<string> errors)
        : base(errors.Count() == 1 ? errors.First() : $"DBC file has {errors.Count()} parse error(s): {string.Join("; ", errors)}")
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
