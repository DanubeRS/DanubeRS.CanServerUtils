using System.Text;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.Lib.BinaryLogs;

public class Reader
{
    private readonly Stream _stream;
    private readonly ILogger _logger;
    private uint _logVersion = 0;
    private ulong _lastSyncTime = 0L;

    //Buffers
    private byte[] _timeSyncBuffer = new byte[8];
    private byte[] _frameDataBuffer = new byte[5];

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

    const int HeaderSize = 22;
    private const string v1Header = "CANSERVER_v2_CANSERVER";
    private const string v2Header = "CANSERVER_v3_CANSERVER";

    private async Task<bool> Initialise()
    {
        _stream.Seek(0, SeekOrigin.Begin);
        // Validate Header
        var header = new byte[HeaderSize];
        if (await _stream.ReadAsync(header) != HeaderSize)
            return false;
        var logVersion = LogVersionFromBuffer(header);
        // Log version required at start
        if (logVersion == 0)
            return false;
        _logVersion = logVersion;
        return true;
    }

    private uint LogVersionFromBuffer(byte[] header)
    {
        return Encoding.ASCII.GetString(header) switch
        {
            v1Header => 1,
            v2Header => 2,
            _ => 0,
        };
    }

    public LogReaderFrame? GetNextFrame()
    {
        while (true)
        {
            var frameStart = _stream.ReadByte();
            switch (frameStart)
            {
                // EOF
                case -1: return null;
                // Split; check for new header in the case of a merged file
                case 0x43:
                    var remainingBuffer = new byte[22];
                    remainingBuffer[0] = (byte)frameStart;
                    var read = _stream.Read(remainingBuffer, 1, 21);
                    if (read != 21) throw new Exception("Invalid Frame");
                    var logVersion = LogVersionFromBuffer(remainingBuffer);
                    // No valid header found, so go back
                    if (logVersion == 0)
                    {
                        _stream.Seek(-21, SeekOrigin.Current);
                        continue;
                    }

                    _logVersion = logVersion;
                    break;
            }

            switch (_logVersion)
            {
                case 1: return ReadV1Frame((byte)frameStart);
                case 2: return ReadV2Frame((byte)frameStart);
            }
        }
    }

    private LogReaderFrame? ReadV2Frame(byte start)
    {
        while (true)
        {
            // Time sync frame
            if (start == 0xA0)
            {
                var timeData = new byte[8];
                _stream.ReadExactly(timeData, 0, 8);
                _lastSyncTime = BitConverter.ToUInt64(timeData, 0);
            }
            // CAN Frame
            else if ((start & 0b11110000) == 0xB0)
            {
                var frameData = new byte[5];
                var headerRead = _stream.Read(frameData, 0, 5);
                if (headerRead != 5) return null;

                var frameTimeOffset = (start & 0x0F) + (frameData[0] << 4) + (frameData[1] << 12) +
                                      (((frameData[2] & 0xF8) >> 3) << 20);
                var frameTime = _lastSyncTime + (ulong)frameTimeOffset;

                var frameId = ((frameData[2] & 0x07) << 8) + (frameData[3] & 0xFF);
                var frameLength = frameData[4] & 0x0F;
                frameLength = frameLength switch
                {
                    < 0 => 0,
                    > 8 => 8,
                    _ => frameLength
                };

                var busId = (frameData[4] & 0xF0) >> 4;
                var framePayload = new byte[frameLength];
                var read = _stream.Read(framePayload);
                if (read != frameLength)
                {
                    //TODO
                    return null;
                    throw new Exception("Invalid Frame");
                }

                return new LogReaderFrame(frameTime, (uint)frameId, (uint)busId, framePayload);
            }
            else
            {
                _logger.LogDebug("Skipping frame {byte:x8}", start & 0b11110000);
            }

            var nextStart = _stream.ReadByte();
            if (nextStart == -1)
                return null;
            start = (byte)nextStart;
        }
    }

    private LogReaderFrame? ReadV1Frame(byte start)
    {
        while (true)
        {
            switch (start)
            {
                // Mark
                case 0xcd:
                    throw new NotImplementedException("Marks not supported (yet)");
                // Time sync
                case 0xce:
                    try
                    {
                        _stream.ReadExactly(_timeSyncBuffer, 0, 8);
                    }
                    catch (EndOfStreamException e)
                    {
                        throw new Exception("Time sync payload is the wrong length", e);
                    }

                    _lastSyncTime = BitConverter.ToUInt64(_timeSyncBuffer);
                    start = (byte)_stream.ReadByte();
                    break;
                // Regular payload
                case 0xcf:
                    _stream.ReadExactly(_frameDataBuffer, 0, 5);
                    var frameTimeOffset =
                        BitConverter.ToUInt16(
                            _frameDataBuffer, 0) * 1000;

                    var frameTime = _lastSyncTime + (ulong)frameTimeOffset;

                    var frameId = BitConverter.ToUInt16(_frameDataBuffer, 2);
                    // Length & Bus stored in same byte
                    var frameLength = _frameDataBuffer[4] & 0x0f;
                    var busId = (_frameDataBuffer[4] & 0xf0) >> 4;

                    frameLength = frameLength switch
                    {
                        < 0 => 0,
                        > 8 => 8,
                        _ => frameLength
                    };

                    var framePayload = new byte[frameLength];
                    _stream.ReadExactly(framePayload);
                    return new LogReaderFrame(frameTime, frameId, (uint)busId, framePayload);
            }
        }
    }
}

public record LogReaderFrame(ulong FrameTime, uint FrameId, uint BusId, byte[] FramePayload)
{
}