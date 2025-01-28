// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CommandLine;
using DanubeRS.CANServer.Downloader.BinaryLogs;
using DanubeRS.CANServer.Downloader.Downloader;
using DanubeRS.CANServer.Downloader.Downloader.Client;
using DanubeRS.CANServerUtils.CLI;
using DanubeRS.CanServerUtils.Lib;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

using File = System.IO.File;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<Program>();

var influxClient = new InfluxDBClient("https://influx.internal.danubers.com",
    "AIiYRM3WGnIq6_fERvrqT_Pydm0hB21kePpgJHmEtCn0HV3AyoX0Au3EBfONGMUs86ym4mpztjZV9pDa7j_2oA==");
influxClient.DisableGzip();
const string org = "danubers";
const string bucket = "tesla";
var influxWriteClient = influxClient.GetWriteApiAsync();

var parserResult = Parser.Default.ParseArguments<DownloadOptions, ParseOptions, DownloadAndParseOptions>(args);

async Task<int> DownloadAndParse(DownloadAndParseOptions options)
{
    var archiveDirectory = Path.Combine(options.OutputPath.Replace("~",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "ARCHIVE");
    Directory.CreateDirectory(archiveDirectory);
    var database = await BootstrapDatabase(options);
    var inputFile = new BufferBlock<string>();
    var writeFile = new ActionBlock<string>(async s =>
    {
        var fileInfo = new FileInfo(s);
        await ParseFileInternal(fileInfo, database, influxWriteClient);
        fileInfo.MoveTo(Path.Combine(archiveDirectory, fileInfo.Name));
    }, new ExecutionDataflowBlockOptions()
    {
        MaxDegreeOfParallelism = 1,
    });
    inputFile.LinkTo(writeFile, new DataflowLinkOptions { PropagateCompletion = true });
    await DownloadInternal(options, s => inputFile.Post(s));
    inputFile.Complete();
    await writeFile.Completion;
    return 0;
}

async Task<int> Download(DownloadOptions options)
{
    return await DownloadInternal(options);
}

async Task<int> DownloadInternal(IDownloadOptions downloadOptions, Action<string>? onFileDownloaded = null,
    CancellationToken cancellationToken = default)
{
    var downloader = new Downloader(downloadOptions.Address, loggerFactory.CreateLogger<Downloader>());
    await downloader.DownloadAllFiles(LogFileType.Interval, downloadOptions.Remove, true, downloadOptions.OutputPath,
        cancellationToken, onFileDownloaded);
    return 0;
}

async Task<int> Parse(ParseOptions options)
{
    while (true)
    {
        var database = await BootstrapDatabase(options);
        var archiveDirectory = options.ArchivePath?.Replace("~",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        var inputPath = options.InputPath;
        var inputDir = inputPath == null
            ? new DirectoryInfo(Directory.GetCurrentDirectory())
            : new DirectoryInfo(inputPath.Replace("~",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        var files = inputDir.EnumerateFiles("*.log").OrderBy(f => f.Name);

        foreach (var file in files)
        {
            await ParseFileInternal(file, database, influxWriteClient);
            if (archiveDirectory != null)
                file.MoveTo(Path.Combine(archiveDirectory, Path.Combine(archiveDirectory, file.Name)));
        }

        if (options.Watch <= 0) return 0;

        logger.LogInformation("Parsing is watching, delay {Seconds} before next loop...", options.Watch);
        await Task.Delay(TimeSpan.FromSeconds(options.Watch));
    }

    return 0;
}

parserResult.MapResult(
    (DownloadOptions opts) => Download(opts).GetAwaiter().GetResult(),
    (ParseOptions opts) => Parse(opts).GetAwaiter().GetResult(),
    (DownloadAndParseOptions opts) => DownloadAndParse(opts).GetAwaiter().GetResult(),
    errors => 1
);
return;

async Task<Database> BootstrapDatabase(IParseOptions parseOptions)
{
    var database = new Database(loggerFactory.CreateLogger<Database>());
    foreach (var dbc in parseOptions.Databases)
    {
        await using var dbcStream = File.OpenRead(dbc.Replace("~",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        using var dbcReader = new StreamReader(dbcStream);
        database.AddFile(dbcReader);
    }

    return database;
}

async Task ParseFileInternal(FileInfo fileInfo,
    Database database, WriteApiAsync writeApiAsync)
{
    var stopWatch = new Stopwatch();
    var pointData = new List<PointData>();
    logger.LogInformation("Reading file {LogFileName}", fileInfo.FullName);
    await using var fs = fileInfo.OpenRead();
    var reader = await Reader.Open(fs, loggerFactory.CreateLogger<Reader>());
    if (reader == null)
    {
        logger.LogWarning("Could not open file {LogFileName}", fileInfo.FullName);
        return;
    }

    var frames = 0L;
    var points = 0L;
    stopWatch.Restart();
    var initialFrameTimeLogged = false;
    var pointsStopwatch = Stopwatch.StartNew();
    while (true)
    {
        var frame = reader.GetNextFrame();
        if (frame == null)
            break;
        logger.LogDebug("Decoding frame {FrameId:x8} @ {FrameTime}", frame.FrameId, frame.FrameTime);
        if (database.TryParseBinaryMessage(frame.FrameId, frame.FramePayload, out var value, out var defn))
        {
            // if (frame.FrameId != 79) continue;

            logger.LogDebug("Successfully Parsed message #{MessageNumber} @ {MessageTime} (ID:{FrameId})", frames,
                frame.FrameTime, frame.FrameId);
            logger.LogTrace("{MessageId} {MessageName} {Signals}", value.MessageId, value.MessageName,
                value.Signals.Select(s => $"{s.SignalName}"));

            pointData.AddRange(value.Signals
                .Select(signal => new { signal, signalDfn = defn.Signals.First(s => s.Signal.Name == signal.SignalName) })
                .Select(t => PointData.Measurement("CAN")
                    .Tag("_unit", t.signalDfn.Signal.Unit.TrimStart('"').TrimEnd('"'))
                    .Field(t.signal.SignalName, t.signal.Value)
                    .Tag("bus", frame.BusId.ToString())
                    .Tag("frame_id", defn.Header.Id.ToString())
                    .Tag("frame_name", defn.Header.Name)
                    .Timestamp((long)frame.FrameTime / 1000, WritePrecision.Ms)));
        }

        if (pointData.Count > 5_000)
        {
            var rqStop = Stopwatch.StartNew();
            await writeApiAsync.WritePointsAsync(pointData, bucket, org);
            rqStop.Stop();
            logger.LogDebug("Processed {points} points ({pointsSec}/sec) (RQ: {RequestSec})", pointData.Count,
                (pointData.Count / pointsStopwatch.Elapsed.TotalSeconds), rqStop.Elapsed.TotalSeconds);
            pointsStopwatch.Restart();
            points += pointData.Count;
            pointData = new List<PointData>();
        }

        if (!initialFrameTimeLogged)
        {
            logger.LogInformation("First file frame time {Timestamp:s}",
                DateTime.UnixEpoch.AddMicroseconds(frame.FrameTime));
            initialFrameTimeLogged = true;
        }

        frames++;
        if (frames % 10_000 == 0)
        {
            logger.LogDebug("Current frame time {Timestamp:s}",
                DateTime.UnixEpoch.AddMicroseconds(frame.FrameTime));
            logger.LogDebug("Processed {frames} frames ({framesSec}/sec)", frames,
                (frames / stopWatch.Elapsed.TotalSeconds));
        }
    }

    points += pointData.Count;
    await writeApiAsync.WritePointsAsync(pointData, "tesla", "danubers");
    logger.LogDebug("Processed {points} points ({pointsSec}/sec)", points,
        (points / stopWatch.Elapsed.TotalSeconds));

    stopWatch.Stop();
    logger.LogInformation("Processed {frames} frames from {path} in {elapsed} ({framesSec}/sec)", frames, fileInfo.Name,
        stopWatch.Elapsed.TotalSeconds, frames / stopWatch.Elapsed.TotalSeconds);
    logger.LogInformation("Processed {points} points in {elapsed}({pointsSec}/sec)", points,
        stopWatch.Elapsed.TotalSeconds,
        (points / stopWatch.Elapsed.TotalSeconds));
}