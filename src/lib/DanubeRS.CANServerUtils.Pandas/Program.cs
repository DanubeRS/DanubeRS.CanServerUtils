// See https://aka.ms/new-console-template for more information

using System.Globalization;
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

var factory = new PandasClientFactory("192.168.8.243", 1338, loggerFactory);

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

var db = await BootstrapDatabase(["~/CANTEST/Model3CAN.dbc", "~/CANTEST/CANServer.dbc"]);

void HandleMessages(PandasMessage message)
{
    logger.LogInformation("Received {Message} at {timestamp:s}", message.Frames.Length, message.Timestamp);
    foreach (var frame in message.Frames)
    {
        logger.LogInformation("Frame {FrameId}", frame.FrameId);
        if (db.TryParseBinaryMessage(frame.FrameId, frame.FrameData, out var messageValue, out var messageDefinition))
        {
            logger.LogInformation("Decoded {MessageName}", messageDefinition.Header.Name);
            foreach (var signal in messageDefinition.Signals)
            {
                var value = messageValue.Signals.Single(s => s.SignalName == signal.Signal.Name).Value;
                var signalValueName =
                    signal.ValueLookup?.GetValueOrDefault((uint)value) ?? value.ToString(CultureInfo.InvariantCulture);
                logger.LogInformation("{Name} - {Value}", signal.Signal.Name, signalValueName);
            }
        }
    }
}

var instance = await factory.CreateAsync(HandleMessages, CancellationToken.None);
// await instance.Track((0x0F, [0x05, 0x00]), (0x0F, [0x05, 0x02]), (0x01, [0x02, 0x01]));
await instance.Handle;