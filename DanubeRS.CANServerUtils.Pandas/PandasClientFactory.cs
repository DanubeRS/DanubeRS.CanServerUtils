using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace DanubeRS.CANServerUtils.Pandas;

public class PandasClientFactory
{
    private readonly string _url;
    private readonly int _port;

    public PandasClientFactory(string url, int port)
    {
        _url = url;
        _port = port;
    }

    public async Task<IPandasClientInstance> CreateAsync(Action<PandasMessage> onMessageReceived,
        CancellationToken cancellationToken)
    {
        var instance = new PandasClientInstance(_url, _port);
        instance.StartListening(cancellationToken, onMessageReceived);
        return instance;
    }

    private class PandasClientInstance : IPandasClientInstance
    {
        private readonly UdpClient _client;
        private Task _heartbeatTask;
        private Task _listenLoop;

        public PandasClientInstance(string address, int port)
        {
            _client = new UdpClient(address, port);
        }

        public void StartListening(CancellationToken cancellationToken, Action<PandasMessage> onMessageReceived)
        {
            _heartbeatTask = HeartbeatLoop(cancellationToken);
            _listenLoop = ReceiveLoop(onMessageReceived, cancellationToken);
        }

        public async Task StopListening()
        {
            await _heartbeatTask;
            await _listenLoop;
        }

        private async Task HeartbeatLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) break;
                await _client.SendAsync(Encoding.UTF8.GetBytes("ehllo"), cancellationToken);
                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task ReceiveLoop(Action<PandasMessage> onMessageReceived, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await _client.ReceiveAsync(cancellationToken);
                var buffer = new byte[4];
                Array.Copy(result.Buffer, buffer, 4);
                var frameIdInt = BitConverter.ToInt32(buffer);
                Array.Copy(result.Buffer, 4, buffer, 0, 4);
                var frameDetailsInt = BitConverter.ToInt32(buffer);
                var frameId = frameIdInt >> 21;
                var frameLength = frameDetailsInt & 0x0F;
                var frameBusId = frameDetailsInt >> 4;
                var frameData = new byte[8];
                Array.Copy(result.Buffer, 8, frameData, 0, 8);
                onMessageReceived(
                    new PandasMessage(new PandasMessageFrame(frameId, frameLength, frameBusId, frameData)));
            }
        }


        public async Task<bool> Track(params (byte busId, byte[] frameId)[] messages)
        {
            var trackerBytes = messages.SelectMany(m => new[] { m.busId, m.frameId[0], m.frameId[1] }).ToArray();
            await _client.SendAsync(new byte[] { 0x0F }.Concat(trackerBytes).ToArray());
            return true;
        }

        public async Task<bool> Untrack(params (byte busId, byte[] frameId)[] messages)
        {
            var trackerBytes = messages.SelectMany(m => new[] { m.busId, m.frameId[0], m.frameId[1] }).ToArray();
            await _client.SendAsync(new byte[] { 0x0E }.Concat(trackerBytes).ToArray());
            return true;
        }

        public Task Handle => _listenLoop;

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

public class PandasMessage(params PandasMessageFrame[] frames)
{
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    public PandasMessageFrame[] Frames { get; } = frames;
}

public class PandasMessageFrame
{
    public int FrameId { get; }
    public int FrameLength { get; }
    public int FrameBusId { get; }
    public byte[] FrameData { get; }

    public PandasMessageFrame(int frameId, int frameLength, int frameBusId, byte[] frameData)
    {
        FrameId = frameId;
        FrameLength = frameLength;
        FrameBusId = frameBusId;
        FrameData = frameData;
    }
}

public interface IPandasClientInstance : IDisposable
{
    Task<bool> Track(params (byte busId, byte[] frameId)[] messages);
    Task<bool> Untrack(params (byte busId, byte[] frameId)[] messages);
    Task Handle { get; }
}