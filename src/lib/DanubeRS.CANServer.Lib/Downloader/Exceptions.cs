using DanubeRS.CANServer.Lib.Downloader.Client;

namespace DanubeRS.CANServer.Lib.Downloader;

public class FileDeletedException : Exception
{
    public Client.LogFileRecord File { get; }

    public FileDeletedException(Client.LogFileRecord file, string message)
        : base(message) => File = file;
}

public class ServerUnavailableException : Exception
{
    public Uri Endpoint { get; }

    public ServerUnavailableException(Uri endpoint, Exception inner)
        : base($"Cannot reach CANServer at {endpoint}", inner)
    {
        Endpoint = endpoint;
    }
}
