using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServerUtils.Pandas;

public class PandasClientFactory
{
    private readonly string _url;
    private readonly int _port;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    public PandasClientFactory(string url, int port, ILoggerFactory loggerFactory)
    {
        _url = url;
        _port = port;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<PandasClientFactory>();
    }

    public async Task<IPandasClientInstance> CreateAsync(Action<PandasMessage> onMessageReceived,
        CancellationToken cancellationToken)
    {
        var instance = new PandasClientInstance(_url, _port, _loggerFactory.CreateLogger<PandasClientInstance>());
        await instance.StartListening(cancellationToken, onMessageReceived);
        return instance;
    }

    private class PandasClientInstance : IPandasClientInstance
    {
        private readonly ILogger _logger;
        private readonly UdpClient _client;
        private Task _heartbeatTask;
        private Task _listenLoop;
        private readonly TaskCompletionSource<bool> _ackCompletionSource;

        public PandasClientInstance(string address, int port, ILogger logger)
        {
            _logger = logger;
            _client = new UdpClient(address, port);
            _ackCompletionSource = new TaskCompletionSource<bool>();
        }

        public async Task StartListening(CancellationToken cancellationToken, Action<PandasMessage> onMessageReceived)
        {
            _heartbeatTask = HeartbeatLoop(cancellationToken);
            _listenLoop = ReceiveLoop(onMessageReceived, cancellationToken);
            // Don't return until the ACK is completed and true
            await _ackCompletionSource.Task;
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
                _logger.LogInformation("Heartbeat sent");
                await Task.Delay(5000, cancellationToken);
            }
        }

        private async Task ReceiveLoop(Action<PandasMessage> onMessagesReceived, CancellationToken cancellationToken)
        {
            var frames = new List<PandasMessageFrame>();
            var buffer = new byte[4];
            while (true)
            {
                var offset = 0;
                cancellationToken.ThrowIfCancellationRequested();
                var result = await _client.ReceiveAsync(cancellationToken);
                while (offset < result.Buffer.Length)
                {
                    Array.Clear(buffer);
                    // Get frame
                    Array.Copy(result.Buffer, buffer, 4);
                    var frameIdInt = BitConverter.ToInt32(buffer);
                    var frameId = frameIdInt >> 21;
                    
                    // Get frame data
                    Array.Copy(result.Buffer, 4, buffer, 0, 4);
                    var frameDetailsInt = BitConverter.ToInt32(buffer);
                    var frameLength = frameDetailsInt & 0x0F;
                    var frameBusId = frameDetailsInt >> 4;
                    
                    // Build frame
                    var frameData = new byte[8];
                    Array.Copy(result.Buffer, 8, frameData, 0, 8);
                    var pandasMessageFrame = new PandasMessageFrame(frameId, frameLength, frameBusId, frameData);
                    frames.Add(pandasMessageFrame);
                    
                    // If we have an ACK packet, then the client is now ready to accept streamed frames
                    if (!_ackCompletionSource.Task.IsCompleted && frameId == 6)
                    {
                        _logger.LogInformation("ACK received");
                        _ackCompletionSource.TrySetResult(true);
                    }

                    offset += 16;
                }

                onMessagesReceived(new PandasMessage(frames.ToArray()));
                frames.Clear();
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

public class PandasMessageFrame(int frameId, int frameLength, int frameBusId, byte[] frameData)
{
    public int FrameId { get; } = frameId;
    public int FrameLength { get; } = frameLength;
    public int FrameBusId { get; } = frameBusId;
    public byte[] FrameData { get; } = frameData;
}

public interface IPandasClientInstance : IDisposable
{
    Task<bool> Track(params (byte busId, byte[] frameId)[] messages);
    Task<bool> Untrack(params (byte busId, byte[] frameId)[] messages);
    Task Handle { get; }
}