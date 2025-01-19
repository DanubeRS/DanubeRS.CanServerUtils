// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Text;
using DanubeRS.CanServerUtils.Lib.DBC;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<Program>();

var client = new UdpClient();
client.Connect("192.168.8.243", 1338);
var taskFactory = new TaskFactory();

async Task HeartbeatLoop()
{
    while (true)
    {
        await client.SendAsync(Encoding.UTF8.GetBytes("ehllo"));
        await Task.Delay(1000);
    }
}

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
        logger.LogInformation("{Name} {Value} {Unit}", signal.Name, value.Signals.Single(s => s.SignalName ==signal.Name).Value, signal.Unit);
    }
}

async Task ReceiveLoop()
{
    while (true)
    {
        var result = await client.ReceiveAsync();
        var buffer = new byte[4];
        Array.Copy(result.Buffer, buffer, 4);
        var frameIdInt = BitConverter.ToInt32(buffer);
        Array.Copy(result.Buffer, 4, buffer, 0, 4);
        var frameDetailsInt = BitConverter.ToInt32(buffer);
        var frameId = frameIdInt >> 21;
        var frameLength = frameDetailsInt & 0x0F;
        var frameBusId = frameDetailsInt >> 4;
        var frameData = new byte[8];
        Array.Copy(result.Buffer, 8, frameData, 0, 8);
        DecodeFrame(frameBusId, frameId, frameData);
        
    }
}

var heartbeatTask = Task.Factory.StartNew(HeartbeatLoop);
var receiveTask = await Task.Factory.StartNew(ReceiveLoop);
await client.SendAsync(new byte[] { 0x0F, 0x01, 0x02, 0x0C });
await heartbeatTask;
await receiveTask;