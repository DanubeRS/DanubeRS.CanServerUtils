using System.Buffers;
using System.Text;
using DanubeRS.CANServer.Lib.Downloader;
using DanubeRS.CANServer.Lib.Pandas;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.Lib.BinaryLogs;

public class Reader : IAsyncDisposable, IDisposable
{
    private readonly Stream _stream;
    private readonly ILogger _logger;
    private uint _logVersion = 0;
    private ulong _lastSyncTime = 0L;
    private bool _disposed = false;

    public static async Task<Reader?> Open(Stream stream, ILogger logger)
    {
        var reader = new Reader(stream, logger);
        return await reader.Initialise() ? reader : null;
    }

    private Reader(Stream stream, ILogger logger)
    {
        _stream = stream;
        _logger = logger;
    }

    private const int HeaderSize = 22;
    private const string Version1Header = "CANSERVER_v2_CANSERVER";
    private const string Version2Header = "CANSERVER_v3_CANSERVER";

    private async Task<bool> Initialise()
    {
        _stream.Seek(0, SeekOrigin.Begin);
        var header = new byte[HeaderSize];
        if (await _stream.ReadAsync(header) != HeaderSize)
            return false;
        var logVersion = LogVersionFromBuffer(header);
        if (logVersion == 0)
            return false;
        _logVersion = logVersion;
        return true;
    }

    private static uint LogVersionFromBuffer(ReadOnlySpan<byte> header)
    {
        return Encoding.ASCII.GetString(header) switch
        {
            Version1Header => 1,
            Version2Header => 2,
            _ => 0,
        };
    }

    public LogReaderFrame? GetNextFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        while (true)
        {
            var frameStart = _stream.ReadByte();
            switch (frameStart)
            {
                case -1:
                    return null;
                case 0x43:
                    Span<byte> splitHeader = stackalloc byte[HeaderSize];
                    splitHeader[0] = (byte)frameStart;
                    var read = _stream.Read(splitHeader[1..]);
                    if (read != HeaderSize - 1)
                        throw new InvalidFrameException("Invalid split header", _stream.Position);
                    var logVersion = LogVersionFromBuffer(splitHeader);
                    if (logVersion == 0)
                    {
                        _stream.Seek(-(HeaderSize - 1), SeekOrigin.Current);
                        continue;
                    }
                    _logVersion = logVersion;
                    break;
                default:
                    break;
            }

            switch (_logVersion)
            {
                case 1: return ReadV1Frame((byte)frameStart);
                case 2: return ReadV2Frame((byte)frameStart);
                default: return null;
            }
        }
    }

    public async Task<LogReaderFrame?> GetNextFrameAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var buffer = ArrayPool<byte>.Shared.Rent(HeaderSize);
        try
        {
            while (true)
            {
                var singleByte = buffer.AsMemory(0, 1);
                var read = await _stream.ReadAsync(singleByte, cancellationToken);
                if (read == 0) return null;
                var frameStart = buffer[0];

                switch (frameStart)
                {
                    case 0x43:
                        var splitHeader = buffer.AsMemory(0, HeaderSize);
                        splitHeader.Span[0] = (byte)frameStart;
                        await _stream.ReadExactlyAsync(splitHeader[1..], cancellationToken);
                        var logVersion = LogVersionFromBuffer(splitHeader.Span);
                        if (logVersion == 0)
                        {
                            _stream.Seek(-(HeaderSize - 1), SeekOrigin.Current);
                            continue;
                        }
                        _logVersion = logVersion;
                        break;
                }

                switch (_logVersion)
                {
                    case 1: return await ReadV1FrameAsync((byte)frameStart, buffer, cancellationToken);
                    case 2: return await ReadV2FrameAsync((byte)frameStart, buffer, cancellationToken);
                    default: return null;
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private async Task<LogReaderFrame?> ReadV2FrameAsync(byte start, byte[] buffer, CancellationToken cancellationToken)
    {
        while (true)
        {
            if (start == 0xA0)
            {
                var timeSyncMem = buffer.AsMemory(0, 8);
                await _stream.ReadExactlyAsync(timeSyncMem, cancellationToken);
                _lastSyncTime = BitConverter.ToUInt64(timeSyncMem.Span);
            }
            else if ((start & 0b11110000) == 0xB0)
            {
                var frameDataMem = buffer.AsMemory(0, 5);
                var headerRead = await _stream.ReadAsync(frameDataMem, cancellationToken);
                if (headerRead != 5) return null;

                var frameTimeOffset = (start & 0x0F) + (buffer[0] << 4) + (buffer[1] << 12) +
                                       (((buffer[2] & 0xF8) >> 3) << 20);
                var frameTime = _lastSyncTime + (ulong)frameTimeOffset;

                var frameId = ((buffer[2] & 0x07) << 8) + (buffer[3] & 0xFF);
                byte frameLength = (byte)Math.Clamp(buffer[4] & 0x0F, 0, 8);

                var busId = (buffer[4] & 0xF0) >> 4;
                var framePayload = new byte[frameLength];
                var read = await _stream.ReadAsync(framePayload, cancellationToken);
                if (read != frameLength)
                    throw new EndOfStreamException(
                        $"Expected {frameLength} bytes but only read {read} at stream position {_stream.Position}");

                return new LogReaderFrame(frameTime, (uint)frameId, (uint)busId, framePayload);
            }
            else
            {
                _logger.LogDebug("Skipping unknown v2 frame byte 0x{Byte:X2}", start);
            }

            var nextByte = buffer.AsMemory(0, 1);
            var nextRead = await _stream.ReadAsync(nextByte, cancellationToken);
            if (nextRead == 0) return null;
            start = buffer[0];
        }
    }

    private async Task<LogReaderFrame?> ReadV1FrameAsync(byte start, byte[] buffer, CancellationToken cancellationToken)
    {
        while (true)
        {
            switch (start)
            {
                case 0xcd:
                    throw new NotImplementedException("Marks not supported (yet)");
                case 0xce:
                    try
                    {
                        var timeSyncMem = buffer.AsMemory(0, 8);
                        await _stream.ReadExactlyAsync(timeSyncMem, cancellationToken);
                    }
                    catch (EndOfStreamException e)
                    {
                        throw new InvalidFrameException("Time sync payload is the wrong length", _stream.Position, e);
                    }

                    _lastSyncTime = BitConverter.ToUInt64(buffer, 0);
                    var nextByte = buffer.AsMemory(0, 1);
                    var read = await _stream.ReadAsync(nextByte, cancellationToken);
                    if (read == 0) return null;
                    start = buffer[0];
                    break;
                case 0xcf:
                    var frameDataMem = buffer.AsMemory(0, 5);
                    await _stream.ReadExactlyAsync(frameDataMem, cancellationToken);

                    var frameTimeOffset =
                        BitConverter.ToUInt16(buffer, 0) * 1000;

                    var frameTime = _lastSyncTime + (ulong)frameTimeOffset;

                    var frameId = BitConverter.ToUInt16(buffer, 2);
                    byte frameLength = (byte)Math.Clamp(buffer[4] & 0x0F, 0, 8);
                    var busId = (buffer[4] & 0xF0) >> 4;

                    var framePayload = new byte[frameLength];
                    await _stream.ReadExactlyAsync(framePayload, cancellationToken);
                    return new LogReaderFrame(frameTime, frameId, (uint)busId, framePayload);
                default:
                    _logger.LogDebug("Skipping unknown v1 frame byte 0x{Byte:X2}", start);
                    var next = buffer.AsMemory(0, 1);
                    var nextRead = await _stream.ReadAsync(next, cancellationToken);
                    if (nextRead == 0) return null;
                    start = buffer[0];
                    break;
            }
        }
    }

    private LogReaderFrame? ReadV2Frame(byte start)
    {
        Span<byte> timeSyncBuffer = stackalloc byte[8];
        Span<byte> frameDataBuffer = stackalloc byte[5];

        while (true)
        {
            if (start == 0xA0)
            {
                _stream.ReadExactly(timeSyncBuffer);
                _lastSyncTime = BitConverter.ToUInt64(timeSyncBuffer);
            }
            else if ((start & 0b11110000) == 0xB0)
            {
                var headerRead = _stream.Read(frameDataBuffer);
                if (headerRead != 5) return null;

                var frameTimeOffset = (start & 0x0F) + (frameDataBuffer[0] << 4) + (frameDataBuffer[1] << 12) +
                                      (((frameDataBuffer[2] & 0xF8) >> 3) << 20);
                var frameTime = _lastSyncTime + (ulong)frameTimeOffset;

                var frameId = ((frameDataBuffer[2] & 0x07) << 8) + (frameDataBuffer[3] & 0xFF);
                byte frameLength = (byte)Math.Clamp(frameDataBuffer[4] & 0x0F, 0, 8);

                var busId = (frameDataBuffer[4] & 0xF0) >> 4;
                var framePayload = new byte[frameLength];
                var read = _stream.Read(framePayload);
                if (read != frameLength)
                    throw new EndOfStreamException(
                        $"Expected {frameLength} bytes but only read {read} at stream position {_stream.Position}");

                return new LogReaderFrame(frameTime, (uint)frameId, (uint)busId, framePayload);
            }
            else
            {
                _logger.LogDebug("Skipping unknown v2 frame byte 0x{Byte:X2}", start);
            }

            var nextStart = _stream.ReadByte();
            if (nextStart == -1)
                return null;
            start = (byte)nextStart;
        }
    }

    private LogReaderFrame? ReadV1Frame(byte start)
    {
        Span<byte> timeSyncBuffer = stackalloc byte[8];
        Span<byte> frameDataBuffer = stackalloc byte[5];

        while (true)
        {
            switch (start)
            {
                case 0xcd:
                    throw new NotImplementedException("Marks not supported (yet)");
                case 0xce:
                    try
                    {
                        _stream.ReadExactly(timeSyncBuffer);
                    }
                    catch (EndOfStreamException e)
                    {
                        throw new InvalidFrameException("Time sync payload is the wrong length", _stream.Position, e);
                    }

                    _lastSyncTime = BitConverter.ToUInt64(timeSyncBuffer);
                    var nextByte = _stream.ReadByte();
                    if (nextByte == -1) return null;
                    start = (byte)nextByte;
                    break;
                case 0xcf:
                    _stream.ReadExactly(frameDataBuffer);
                    var frameTimeOffset =
                        BitConverter.ToUInt16(frameDataBuffer) * 1000;

                    var frameTime = _lastSyncTime + (ulong)frameTimeOffset;

                    var frameId = BitConverter.ToUInt16(frameDataBuffer[2..]);
                    byte frameLength = (byte)Math.Clamp(frameDataBuffer[4] & 0x0F, 0, 8);
                    var busId = (frameDataBuffer[4] & 0xF0) >> 4;

                    var framePayload = new byte[frameLength];
                    _stream.ReadExactly(framePayload);
                    return new LogReaderFrame(frameTime, frameId, (uint)busId, framePayload);
                default:
                    _logger.LogDebug("Skipping unknown v1 frame byte 0x{Byte:X2}", start);
                    var next = _stream.ReadByte();
                    if (next == -1) return null;
                    start = (byte)next;
                    break;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
        await _stream.DisposeAsync();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
        _stream.Dispose();
    }
}

public record LogReaderFrame(ulong FrameTime, uint FrameId, uint BusId, byte[] FramePayload);