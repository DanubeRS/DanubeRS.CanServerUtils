using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.Lib.Pandas;

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
        CancellationToken cancellationToken = default)
    {
        var instance = new PandasClientInstance(_url, _port, _loggerFactory.CreateLogger<PandasClientInstance>());
        await instance.StartListening(cancellationToken, onMessageReceived);
        return instance;
    }

    private class PandasClientInstance : IPandasClientInstance
    {
        private readonly ILogger _logger;
        private readonly UdpClient _client;
        private Task _heartbeatTask = Task.CompletedTask;
        private Task _listenLoop = Task.CompletedTask;
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
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _client.SendAsync("ehllo"u8.ToArray(), cancellationToken);
                    _logger.LogDebug("Heartbeat sent");
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed to send heartbeat");
                }
                await Task.Delay(5000, cancellationToken);
            }
        }

        private async Task ReceiveLoop(Action<PandasMessage> onMessagesReceived, CancellationToken cancellationToken)
        {
            var frames = new List<PandasMessageFrame>();
            var buffer = new byte[4];
            while (!cancellationToken.IsCancellationRequested)
            {
                var offset = 0;
                UdpReceiveResult result;
                try
                {
                    result = await _client.ReceiveAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed to receive messages");
                    continue;
                }
                _logger.Log(LogLevel.Trace, "Result: {Bytes}", BytesToString(result.Buffer));
                while (offset < result.Buffer.Length)
                {
                    Array.Clear(buffer);
                    // Get frame
                    Array.Copy(result.Buffer, offset, buffer, 0, 4);
                    _logger.Log(LogLevel.Debug, "Recieved FrameId: {Bytes}", BytesToString(buffer));
                    var frameIdInt = BitConverter.ToUInt32(buffer);
                    var frameId = frameIdInt >> 21;

                    // Get frame data
                    Array.Copy(result.Buffer, offset + 4, buffer, 0, 4);
                    var frameDetailsInt = BitConverter.ToUInt32(buffer);
                    _logger.Log(LogLevel.Trace, "FrameDetails: {Bytes}", BytesToString(buffer));
                    var frameLength = frameDetailsInt & 0x0F;
                    var frameBusId = frameDetailsInt >> 4;

                    // Build frame
                    var frameData = new byte[8];
                    Array.Copy(result.Buffer, offset + 8, frameData, 0, 8);
                    _logger.Log(LogLevel.Trace, "FrameData: {Bytes}", BytesToString(frameData));
                    var pandasMessageFrame = new PandasMessageFrame(frameId, frameLength, frameBusId, frameData);
                    frames.Add(pandasMessageFrame);

                    // If we have an ACK packet, then the client is now ready to accept streamed frames
                    if (!_ackCompletionSource.Task.IsCompleted && frameId == 6)
                    {
                        _logger.LogDebug("ACK received, marking as ready for listening!");
                        _ackCompletionSource.TrySetResult(true);
                    }

                    offset += 16;
                }

                onMessagesReceived(new PandasMessage(frames.ToArray()));
                frames.Clear();
            }
        }

        private static string BytesToString(byte[] buffer) =>
            string.Join(' ', buffer.Select(b => b.ToString("X2")).ToArray());

        private static class TrackerBytes
        {
            public const byte Track = 0x0F;
            public const byte Untrack = 0x0E;
        }

        public async Task<bool> Track(params (byte busId, (byte first, byte second) frameId)[] messages)
        {
            return await SendTrackerPayload(messages, TrackerBytes.Track);
        }
        public async Task<bool> Untrack(params (byte busId, (byte first, byte second) frameId)[] messages)
        {
            return await SendTrackerPayload(messages, TrackerBytes.Untrack);
        }
        private async Task<bool> SendTrackerPayload((byte busId, (byte first, byte second) frameId)[] messages,
            byte trackerFlag)
        {
            var trackerBytes = messages.SelectMany(m => new[] { m.busId, m.frameId.first, m.frameId.second }).ToArray();
            await _client.SendAsync(new byte[] { trackerFlag }.Concat(trackerBytes).ToArray());
            return true;
        }


        public Task AliveHandle => _listenLoop;

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

public record PandasMessage(params PandasMessageFrame[] Frames)
{
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}

public record PandasMessageFrame(uint FrameId, uint FrameLength, uint FrameBusId, byte[] FrameData);

public interface IPandasClientInstance : IDisposable
{
    Task<bool> Track(params (byte busId, (byte first, byte second) frameId)[] messages);
    Task<bool> Untrack(params (byte busId, (byte first, byte second) frameId)[] messages);
    Task AliveHandle { get; }
}