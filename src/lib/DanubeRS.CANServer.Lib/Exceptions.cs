namespace DanubeRS.CANServer.Lib;

public class CorruptedLogException : Exception
{
    public long Position { get; }

    public CorruptedLogException(string message, long position)
        : base(message) => Position = position;
}

public class UnsupportedLogFormatException : Exception
{
    public uint DetectedVersion { get; }

    public UnsupportedLogFormatException(uint version)
        : base($"Unsupported log format version: {version}")
    {
        DetectedVersion = version;
    }
}
