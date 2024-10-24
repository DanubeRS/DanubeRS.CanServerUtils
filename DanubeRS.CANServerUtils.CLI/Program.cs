// See https://aka.ms/new-console-template for more information

using DanubeRS.CanServerUtils.Lib.BinaryLogs;
using DanubeRS.CanServerUtils.Lib.DBC;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(b => { 
    b.AddConsole();
});

var dbc = new Database();
await using var dbcStream = File.OpenRead("./database.dbc");
using var dbcReader = new StreamReader(dbcStream);
dbc.AddFile(dbcReader);

var logger = loggerFactory.CreateLogger<Program>();
var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
var files = dir.EnumerateFiles("*.log");
foreach (var file in files)
{
    await using var fs = file.OpenRead();
    var reader = await Reader.Open(fs, loggerFactory.CreateLogger<Reader>());
    var frames = 0;
    while (true)
    {
        var frame = reader.GetNextFrame();
        if (frame == null)
            return;
        if (dbc.TryParseBinaryMessage(frame.FrameId, frame.FramePayload, out var value))
        {
          logger.LogDebug("Successfully Parsed message #{MessageNumber} @ {MessageTime} (ID:{FrameId})", frames, frame.FrameTime, frame.FrameId);
          logger.LogTrace("{MessageId} {MessageName} {Signals}", value.MessageId, value.MessageName, value.Signals.Select(s => $"{s.SignalName}"));
        }
        frames++;
        if (frames % 1000 == 0)
            logger.LogInformation("Processed {frames} frames", frames);
    }
}