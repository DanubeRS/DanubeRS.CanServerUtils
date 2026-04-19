namespace DanubeRS.CANServer.Lib.Connections;

public interface ICanConnection : IAsyncDisposable, IDisposable
{
    bool IsConnected { get; }
    ConnectionInfo Info { get; }
    event EventHandler<CanFrame>? FrameReceived;
    event EventHandler<bool>? ConnectionChanged;
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    Task TrackAsync(uint frameId, byte busId = 0x01, CancellationToken cancellationToken = default);
    Task UntrackAsync(uint frameId, byte busId = 0x01, CancellationToken cancellationToken = default);
}