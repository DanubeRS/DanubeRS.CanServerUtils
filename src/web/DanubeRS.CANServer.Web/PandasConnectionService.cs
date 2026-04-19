using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using DanubeRS.CANServer.Lib.Connections;
using DanubeRS.CanServerUtils.Lib;
using DanubeRS.DBCParser;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.Web;

public class PandasConnectionService
{
    private readonly ILogger<PandasConnectionService> _logger;
    private readonly DbcEditService _dbcService;
    private PandasDriver? _driver;
    private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();

    public bool IsConnected => _driver?.IsConnected ?? false;
    public Lib.Connections.ConnectionInfo? Info => _driver?.Info;
    public int ClientCount => _clients.Count;

    public PandasConnectionService(ILogger<PandasConnectionService> logger, DbcEditService dbcService)
    {
        _logger = logger;
        _dbcService = dbcService;
    }

    public async Task ConnectAsync(string ip, int port, CancellationToken cancellationToken = default)
    {
        if (_driver != null)
            await DisconnectAsync(cancellationToken);

        var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());
        _driver = new PandasDriver(ip, port, loggerFactory);
        _driver.FrameReceived += OnFrameReceived;
        _driver.ConnectionChanged += OnConnectionChanged;

        try
        {
            await _driver.ConnectAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Panda at {Ip}:{Port}", ip, port);
            _driver.FrameReceived -= OnFrameReceived;
            _driver.ConnectionChanged -= OnConnectionChanged;
            _driver.Dispose();
            _driver = null;
            throw;
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_driver != null)
        {
            _driver.FrameReceived -= OnFrameReceived;
            _driver.ConnectionChanged -= OnConnectionChanged;
            await _driver.DisconnectAsync(cancellationToken);
            _driver.Dispose();
            _driver = null;
        }
    }

    public async Task TrackAsync(uint frameId, byte busId = 0x01, CancellationToken cancellationToken = default)
    {
        if (_driver != null)
            await _driver.TrackAsync(frameId, busId, cancellationToken);
    }

    public async Task UntrackAsync(uint frameId, byte busId = 0x01, CancellationToken cancellationToken = default)
    {
        if (_driver != null)
            await _driver.UntrackAsync(frameId, busId, cancellationToken);
    }

    public Guid AddClient(WebSocket webSocket)
    {
        var id = Guid.NewGuid();
        _clients[id] = webSocket;
        return id;
    }

    public void RemoveClient(Guid clientId)
    {
        if (_clients.TryRemove(clientId, out var webSocket))
            _ = CleanupClientAsync(webSocket);
    }

    private static async Task CleanupClientAsync(WebSocket webSocket)
    {
        try
        {
            if (webSocket.State == WebSocketState.Open)
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected", CancellationToken.None);
        }
        catch { }
    }

    private void OnFrameReceived(object? sender, CanFrame frame)
    {
        var message = DecodeFrame(frame);
        _ = BroadcastAsync(message);
    }

    private void OnConnectionChanged(object? sender, bool connected)
    {
        var status = new { type = "status", connected };
        _ = BroadcastAsync(status);
    }

    private object DecodeFrame(CanFrame frame)
    {
        if (!_dbcService.IsLoaded)
        {
            return new
            {
                type = "frame",
                frameId = frame.FrameId,
                timestamp = frame.Timestamp.ToUnixTimeMilliseconds(),
                rawData = frame.Data
            };
        }

        var defn = _dbcService.GetMessageDefinition(frame.FrameId);
        if (defn == null)
        {
            return new
            {
                type = "frame",
                frameId = frame.FrameId,
                timestamp = frame.Timestamp.ToUnixTimeMilliseconds(),
                rawData = frame.Data
            };
        }

        var bitArray = new System.Collections.BitArray(frame.Data);
        var success = defn.IsMultiplexed
            ? CanServerUtils.Lib.Decoder.TryParseMultiplexedMessage(defn, bitArray, out var value)
            : CanServerUtils.Lib.Decoder.TryParseRegularMessage(defn, bitArray, out value);

        if (!success || value == null)
        {
            return new
            {
                type = "frame",
                frameId = frame.FrameId,
                timestamp = frame.Timestamp.ToUnixTimeMilliseconds(),
                rawData = frame.Data
            };
        }

        return new
        {
            type = "frame",
            frameId = frame.FrameId,
            timestamp = frame.Timestamp.ToUnixTimeMilliseconds(),
            signals = value.Signals.Select(s => new { name = s.SignalName, value = s.Value }).ToArray()
        };
    }

    private async Task BroadcastAsync(object message)
    {
        if (_clients.IsEmpty) return;

        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);
        var deadClients = new List<Guid>();

        foreach (var (id, client) in _clients)
        {
            try
            {
                if (client.State == WebSocketState.Open)
                    await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                else
                    deadClients.Add(id);
            }
            catch (WebSocketException)
            {
                deadClients.Add(id);
            }
        }

        foreach (var deadId in deadClients)
        {
            if (_clients.TryRemove(deadId, out var dead))
                _ = CleanupClientAsync(dead);
        }
    }
}