namespace DanubeRS.CANServer.Lib.Connections;

public record CanFrame(uint FrameId, uint BusId, byte[] Data, DateTimeOffset Timestamp);