// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Text;
using DanubeRS.CanServerUtils.Lib.DBC;
using DanubeRS.CANServerUtils.Pandas;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Debug);
});

var logger = loggerFactory.CreateLogger<Program>();

var factory = new PandasClientFactory("192.168.8.243", 1338);

async Task<Database> BootstrapDatabase(string[] dbs)
{
    var database = new Database(loggerFactory.CreateLogger<Database>());
    foreach (var dbc in dbs)
    {
        await using var dbcStream = File.OpenRead(dbc.Replace("~",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        using var dbcReader = new StreamReader(dbcStream);
        database.AddFile(dbcReader);
    }

    return database;
}

var db = await BootstrapDatabase(["~/Model3CAN.dbc"]);

void DecodeFrame(int busId, int frameId, byte[] frameData)
{
    if (!db.TryParseBinaryMessage(frameId, frameData, out var value, out var defn)) return;

    foreach (var signal in defn.Signals.Where(s => s.Name == "VCRIGHT_wattsDemandEvap"))
    {
        logger.LogInformation("{Name} {Value} {Unit}", signal.Name,
            value.Signals.Single(s => s.SignalName == signal.Name).Value, signal.Unit);
    }
}

void HandleMessage(PandasMessage message)
{
    logger.LogInformation("Received at {timestamp:s}", message.Timestamp);
    foreach (var frame in message.Frames)
        if (db.TryParseBinaryMessage(frame.FrameId, frame.FrameData, out var value, out var defn))
        {
            logger.LogInformation("Decoded {MessageName}", defn.Header.Name);
            foreach (var signal in defn.Signals)
            {
                logger.LogInformation("{Name} - {Value}", signal.Name, value.Signals.Single(s => s.SignalName == signal.Name).Value);
            }
        }
}

var instance = await factory.CreateAsync(HandleMessage, CancellationToken.None);
await instance.Track((0x01, [0x01, 0x32]));
await instance.Handle;