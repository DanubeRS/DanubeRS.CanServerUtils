using DanubeRS.CANServer.Lib.Pandas;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.Lib.Connections;

public class PandasDriver : ICanConnection
{
    private readonly string _ip;
    private readonly int _port;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<PandasDriver> _logger;
    private IPandasClientInstance? _instance;
    private CancellationTokenSource? _cts;

    public bool IsConnected => _instance != null && !(_cts?.IsCancellationRequested ?? true);

    public ConnectionInfo Info => new("Pandas", _ip, _port, IsConnected);

    public event EventHandler<CanFrame>? FrameReceived;
    public event EventHandler<bool>? ConnectionChanged;

    public PandasDriver(string ip, int port, ILoggerFactory loggerFactory)
    {
        _ip = ip;
        _port = port;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<PandasDriver>();
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_instance != null)
            await DisconnectAsync(cancellationToken);

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var factory = new PandasClientFactory(_ip, _port, _loggerFactory);
        var instance = await factory.CreateAsyncNonBlocking(HandleMessage, _cts.Token);

        lock (this)
        {
            _instance = instance;
        }

        ConnectionChanged?.Invoke(this, true);
        _logger.LogInformation("Connected to Panda at {Ip}:{Port}", _ip, _port);

        _ = Task.Run(async () =>
        {
            try
            {
                await instance.AliveHandle;
                _logger.LogWarning("Panda connection loop ended");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Panda connection loop faulted");
            }
            finally
            {
                IPandasClientInstance? current;
                CancellationTokenSource? currentCts;
                lock (this)
                {
                    current = _instance;
                    currentCts = _cts;
                    if (_instance == instance)
                    {
                        _instance = null;
                        _cts = null;
                    }
                }

                currentCts?.Cancel();
                current?.Dispose();
                ConnectionChanged?.Invoke(this, false);
            }
        }, CancellationToken.None);
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        IPandasClientInstance? instance;
        CancellationTokenSource? cts;

        lock (this)
        {
            instance = _instance;
            cts = _cts;
            _instance = null;
            _cts = null;
        }

        if (instance != null)
        {
            cts?.Cancel();

            try
            {
                await instance.AliveHandle.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (TimeoutException) { }
            catch (Exception) { }

            instance.Dispose();
            ConnectionChanged?.Invoke(this, false);
            _logger.LogInformation("Disconnected from Panda");
        }

        cts?.Dispose();
    }

    public async Task TrackAsync(uint frameId, byte busId = 0x01, CancellationToken cancellationToken = default)
    {
        IPandasClientInstance? instance;
        lock (this) { instance = _instance; }
        if (instance == null) return;
        var (first, second) = EncodeFrameId(frameId);
        await instance.Track(new TrackingPacket(busId, (first, second)));
    }

    public async Task UntrackAsync(uint frameId, byte busId = 0x01, CancellationToken cancellationToken = default)
    {
        IPandasClientInstance? instance;
        lock (this) { instance = _instance; }
        if (instance == null) return;
        var (first, second) = EncodeFrameId(frameId);
        await instance.Untrack(new TrackingPacket(busId, (first, second)));
    }

    private void HandleMessage(PandasMessage message)
    {
        foreach (var frame in message.Frames)
        {
            if (frame.FrameId == 6) continue;
            var canFrame = new CanFrame(frame.FrameId, frame.FrameBusId, frame.FrameData, message.Timestamp);
            FrameReceived?.Invoke(this, canFrame);
        }
    }

    private static (byte first, byte second) EncodeFrameId(uint frameId)
    {
        return ((byte)((frameId >> 8) & 0xFF), (byte)(frameId & 0xFF));
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }
}