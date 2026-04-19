namespace DanubeRS.CANServer.Lib.Connections;

public record ConnectionInfo(string DriverType, string? Ip = null, int? Port = null, bool IsConnected = false);