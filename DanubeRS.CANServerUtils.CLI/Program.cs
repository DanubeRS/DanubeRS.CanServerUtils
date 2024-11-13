// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using CommandLine;
using DanubeRS.CANServerUtils.CLI;
using DanubeRS.CanServerUtils.Lib.BinaryLogs;
using DanubeRS.CanServerUtils.Lib.DBC;
using DanubeRS.CanServerUtils.Lib.Downloader;
using DanubeRS.CanServerUtils.Lib.LogFileClient;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Information);
});

var parserResult = Parser.Default.ParseArguments<DownloadOptions, ParseOptions>(args);

async Task<int> Download(DownloadOptions options)
{
    var downloader = new Downloader(options.Address, loggerFactory.CreateLogger<Downloader>());
    await downloader.DownloadAllFiles(LogFileType.Interval, options.Remove, true, options.OutputPath);
    return 0;
}

async Task<int> Parse(ParseOptions options)
{
    var database = new Database();
    foreach (var dbc in options.Databases)
    {
        await using var dbcStream = File.OpenRead(dbc.Replace("~",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        using var dbcReader = new StreamReader(dbcStream);
        database.AddFile(dbcReader);
    }

    var logger = loggerFactory.CreateLogger<Program>();
    var inputPath = options.InputPath;
    var inputDir = inputPath == null
        ? new DirectoryInfo(Directory.GetCurrentDirectory())
        : new DirectoryInfo(inputPath.Replace("~",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
    var files = inputDir.EnumerateFiles("*.log").OrderBy(f => f.Name);

    var sw = new Stopwatch();
    foreach (var file in files)
    {
        await using var fs = file.OpenRead();
        var reader = await Reader.Open(fs, loggerFactory.CreateLogger<Reader>());
        var frames = 0;
        sw.Restart();
        while (true)
        {
            var frame = reader.GetNextFrame();
            if (frame == null)
                break;
            if (database.TryParseBinaryMessage(frame.FrameId, frame.FramePayload, out var value))
            {
                logger.LogDebug("Successfully Parsed message #{MessageNumber} @ {MessageTime} (ID:{FrameId})", frames,
                    frame.FrameTime, frame.FrameId);
                logger.LogTrace("{MessageId} {MessageName} {Signals}", value.MessageId, value.MessageName,
                    value.Signals.Select(s => $"{s.SignalName}"));
            }

            frames++;
            if (frames % 10000 == 0)
                logger.LogDebug("Processed {frames} frames ({framesSec}/sec)", frames, (frames/sw.Elapsed.TotalSeconds));
        }
        
        sw.Stop();
        logger.LogInformation("Processed {frames} frames from {path} in {elapsed} ({framesSec}/sec)", frames, file.Name, sw.Elapsed.TotalSeconds, (frames/sw.Elapsed.TotalSeconds));
    }

    return 0;
}

parserResult.MapResult(
    (DownloadOptions opts) => Download(opts).GetAwaiter().GetResult(),
    (ParseOptions opts) => Parse(opts).GetAwaiter().GetResult(),
    errors => 1
);