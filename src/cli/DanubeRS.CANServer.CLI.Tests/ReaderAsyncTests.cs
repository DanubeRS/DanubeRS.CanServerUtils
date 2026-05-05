using System.Text;
using DanubeRS.CANServer.Lib.BinaryLogs;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CANServer.CLI.Tests;

public class ReaderAsyncTests
{
    private static Stream CreateV2Stream(params byte[] extraFrames)
    {
        var header = Encoding.ASCII.GetBytes("CANSERVER_v3_CANSERVER");
        var ms = new MemoryStream();
        ms.Write(header);
        ms.Write(extraFrames);
        ms.Position = 0;
        return ms;
    }

    private static Stream CreateV1Stream(params byte[] extraFrames)
    {
        var header = Encoding.ASCII.GetBytes("CANSERVER_v2_CANSERVER");
        var ms = new MemoryStream();
        ms.Write(header);
        ms.Write(extraFrames);
        ms.Position = 0;
        return ms;
    }

    private static byte[] CreateV2TimeSyncFrame(ulong syncTime)
    {
        var timeBytes = BitConverter.GetBytes(syncTime);
        var frame = new byte[9];
        frame[0] = 0xA0;
        Array.Copy(timeBytes, 0, frame, 1, 8);
        return frame;
    }

    private static byte[] CreateV2DataFrame(byte start, byte b0, byte b1, byte b2, byte b3, byte b4, byte[] payload)
    {
        var frame = new byte[6 + payload.Length];
        frame[0] = start;
        frame[1] = b0;
        frame[2] = b1;
        frame[3] = b2;
        frame[4] = b3;
        frame[5] = b4;
        Array.Copy(payload, 0, frame, 6, payload.Length);
        return frame;
    }

    private static byte[] CreateV1TimeSyncFrame(ulong syncTime)
    {
        var timeBytes = BitConverter.GetBytes(syncTime);
        var frame = new byte[9];
        frame[0] = 0xce;
        Array.Copy(timeBytes, 0, frame, 1, 8);
        return frame;
    }

    private static byte[] CreateV1DataFrame(ushort timeOffset, ushort frameId, byte lengthAndBus, byte[] payload)
    {
        var frame = new byte[6 + payload.Length];
        frame[0] = 0xcf;
        var timeBytes = BitConverter.GetBytes(timeOffset);
        frame[1] = timeBytes[0];
        frame[2] = timeBytes[1];
        var idBytes = BitConverter.GetBytes(frameId);
        frame[3] = idBytes[0];
        frame[4] = idBytes[1];
        frame[5] = lengthAndBus;
        Array.Copy(payload, 0, frame, 6, payload.Length);
        return frame;
    }

    [Fact]
    public async Task GetNextFrameAsync_V2TimeSyncAndDataFrame_ReturnsFrame()
    {
        var payload = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var timeSync = CreateV2TimeSyncFrame(1000);
        var dataFrame = CreateV2DataFrame(0xB1, 0x00, 0x00, 0x00, 0x00, 0x04, payload);

        await using var stream = CreateV2Stream(timeSync.Concat(dataFrame).ToArray());
        var reader = await Reader.Open(stream, NullLogger.Instance);

        reader.Should().NotBeNull();
        var frame = await reader!.GetNextFrameAsync(CancellationToken.None);

        frame.Should().NotBeNull();
        frame!.FramePayload.Should().Equal(payload);
    }

    [Fact]
    public async Task GetNextFrameAsync_V2StreamEnd_ReturnsNull()
    {
        await using var stream = CreateV2Stream();
        var reader = await Reader.Open(stream, NullLogger.Instance);

        reader.Should().NotBeNull();
        var frame = await reader!.GetNextFrameAsync(CancellationToken.None);

        frame.Should().BeNull();
    }

    [Fact]
    public async Task GetNextFrameAsync_V1TimeSyncAndDataFrame_ReturnsFrame()
    {
        var payload = new byte[] { 0xAA, 0xBB };
        var timeSync = CreateV1TimeSyncFrame(5000);
        var dataFrame = CreateV1DataFrame(100, 0x100, 0x22, payload);

        await using var stream = CreateV1Stream(timeSync.Concat(dataFrame).ToArray());
        var reader = await Reader.Open(stream, NullLogger.Instance);

        reader.Should().NotBeNull();
        var frame = await reader!.GetNextFrameAsync(CancellationToken.None);

        frame.Should().NotBeNull();
        frame!.FramePayload.Should().Equal(payload);
    }

    [Fact]
    public async Task GetNextFrameAsync_V1StreamEnd_ReturnsNull()
    {
        await using var stream = CreateV1Stream();
        var reader = await Reader.Open(stream, NullLogger.Instance);

        reader.Should().NotBeNull();
        var frame = await reader!.GetNextFrameAsync(CancellationToken.None);

        frame.Should().BeNull();
    }

    [Fact]
    public async Task GetNextFrameAsync_MultipleV2Frames_ReturnsAllFrames()
    {
        var payload1 = new byte[] { 0x11 };
        var payload2 = new byte[] { 0x22, 0x33 };

        var timeSync = CreateV2TimeSyncFrame(0);
        var dataFrame1 = CreateV2DataFrame(0xB0, 0x00, 0x00, 0x00, 0x00, 0x01, payload1);
        var dataFrame2 = CreateV2DataFrame(0xB1, 0x00, 0x00, 0x00, 0x00, 0x02, payload2);

        await using var stream = CreateV2Stream(timeSync.Concat(dataFrame1).Concat(dataFrame2).ToArray());
        var reader = await Reader.Open(stream, NullLogger.Instance);

        reader.Should().NotBeNull();

        var frame1 = await reader!.GetNextFrameAsync(CancellationToken.None);
        var frame2 = await reader.GetNextFrameAsync(CancellationToken.None);
        var frame3 = await reader.GetNextFrameAsync(CancellationToken.None);

        frame1.Should().NotBeNull();
        frame1!.FramePayload.Should().Equal(payload1);
        frame2.Should().NotBeNull();
        frame2!.FramePayload.Should().Equal(payload2);
        frame3.Should().BeNull();
    }

    [Fact]
    public async Task GetNextFrameAsync_CancellationRequested_ThrowsTaskCanceledException()
    {
        await using var stream = CreateV2Stream(CreateV2TimeSyncFrame(0));
        var reader = await Reader.Open(stream, NullLogger.Instance);

        reader.Should().NotBeNull();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            reader!.GetNextFrameAsync(cts.Token));
    }

    [Fact]
    public async Task GetNextFrameAsync_V2FrameBusId_ParsedCorrectly()
    {
        var payload = new byte[] { 0x01 };
        var timeSync = CreateV2TimeSyncFrame(0);
        var dataFrame = CreateV2DataFrame(0xB0, 0x00, 0x00, 0x00, 0x00, 0x31, payload);

        await using var stream = CreateV2Stream(timeSync.Concat(dataFrame).ToArray());
        var reader = await Reader.Open(stream, NullLogger.Instance);

        reader.Should().NotBeNull();
        var frame = await reader!.GetNextFrameAsync(CancellationToken.None);

        frame.Should().NotBeNull();
        frame!.BusId.Should().Be(3);
    }
}