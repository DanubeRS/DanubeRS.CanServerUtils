// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CommandLine;
using DanubeRS.CANServerUtils.CLI;
using DanubeRS.CanServerUtils.Lib.BinaryLogs;
using DanubeRS.CanServerUtils.Lib.DBC;
using DanubeRS.CanServerUtils.Lib.Downloader;
using DanubeRS.CanServerUtils.Lib.LogFileClient;
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
    "I8lVgPbbFa820lZfliPYRJEgCR6FGBqSU_2Avx0_2xKV0UnzRpq9KWaybWDImBNlbnv1tUFus0rbmN1IA2NzMA==");
var influxWriteClient = influxClient.GetWriteApiAsync();

var parserResult = Parser.Default.ParseArguments<DownloadOptions, ParseOptions, DownloadAndParseOptions>(args);

async Task<int> DownloadAndParse(DownloadAndParseOptions options)
{
    var archiveDirectory = Path.Combine(options.OutputPath.Replace("~",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "ARCHIVE");
    Directory.CreateDirectory(archiveDirectory);
    var database = await BootstrapDatabase(loggerFactory, options);
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
    var database = await BootstrapDatabase(loggerFactory, options);

    var inputPath = options.InputPath;
    var inputDir = inputPath == null
        ? new DirectoryInfo(Directory.GetCurrentDirectory())
        : new DirectoryInfo(inputPath.Replace("~",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
    var files = inputDir.EnumerateFiles("*.log").OrderBy(f => f.Name);

    foreach (var file in files)
    {
        await ParseFileInternal(file, database, influxWriteClient);
    }

    return 0;
}

parserResult.MapResult(
    (DownloadOptions opts) => Download(opts).GetAwaiter().GetResult(),
    (ParseOptions opts) => Parse(opts).GetAwaiter().GetResult(),
    (DownloadAndParseOptions opts) => DownloadAndParse(opts).GetAwaiter().GetResult(),
    errors => 1
);

async Task<Database> BootstrapDatabase(ILoggerFactory loggerFactory, IParseOptions parseOptions)
{
    var database1 = new Database(loggerFactory.CreateLogger<Database>());
    foreach (var dbc in parseOptions.Databases)
    {
        await using var dbcStream = File.OpenRead(dbc.Replace("~",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        using var dbcReader = new StreamReader(dbcStream);
        database1.AddFile(dbcReader);
    }

    return database1;
}

async Task ParseFileInternal(FileInfo fileInfo,
    Database database, WriteApiAsync writeApiAsync)
{
    var stopWatch = new Stopwatch();
    var pointData = new List<PointData>();
    logger.LogDebug("Reading file {LogFileName}", fileInfo.FullName);
    await using var fs = fileInfo.OpenRead();
    var reader = await Reader.Open(fs, loggerFactory.CreateLogger<Reader>());
    if (reader == null)
    {
        logger.LogWarning("Could not open file {LogFileName}", fileInfo.FullName);
        return;
    }

    var frames = 0;
    stopWatch.Restart();
    while (true)
    {
        var frame = reader.GetNextFrame();
        if (frame == null)
            break;
        logger.LogDebug("Decoding frame {FrameId:x8} @ {FrameTime}", frame.FrameId, frame.FrameTime);
        if (database.TryParseBinaryMessage(frame.FrameId, frame.FramePayload, out var value, out var defn))
        {
            logger.LogDebug("Successfully Parsed message #{MessageNumber} @ {MessageTime} (ID:{FrameId})", frames,
                frame.FrameTime, frame.FrameId);
            logger.LogTrace("{MessageId} {MessageName} {Signals}", value.MessageId, value.MessageName,
                value.Signals.Select(s => $"{s.SignalName}"));

            pointData.AddRange(value.Signals
                .Select(signal => new { signal, signalDfn = defn.Signals.First(s => s.Name == signal.SignalName) })
                .Select(@t => PointData.Measurement("CAN")
                    .Tag("_unit", @t.signalDfn.Unit)
                    .Field(@t.signalDfn.Name, @t.signal.Value)
                    .Tag("bus", frame.BusId.ToString())
                    .Field("frame_id", frame.FrameId)
                    .Timestamp((long)frame.FrameTime / 1000, WritePrecision.Ms)));
        }

        if (pointData.Count / 5_000 > 1)
        {
            await writeApiAsync.WritePointsAsync(pointData, "tesla", "danubers");
            logger.LogDebug("Processed {points} points ({pointsSec}/sec)", pointData.Count,
                (pointData.Count / stopWatch.Elapsed.TotalSeconds));
            pointData.Clear();
        }

        var initialFrameTimeLogged = false;
        if (initialFrameTimeLogged)
        {
            logger.LogInformation("First file frame time {Timestamp:s}",
                DateTime.UnixEpoch.AddMicroseconds(frame.FrameTime));
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

    await writeApiAsync.WritePointsAsync(pointData, "tesla", "danubers");
    logger.LogDebug("Processed {points} points ({pointsSec}/sec)", pointData.Count,
        (pointData.Count / stopWatch.Elapsed.TotalSeconds));

    stopWatch.Stop();
    logger.LogInformation("Processed {frames} frames from {path} in {elapsed} ({framesSec}/sec)", frames, fileInfo.Name,
        stopWatch.Elapsed.TotalSeconds, (frames / stopWatch.Elapsed.TotalSeconds));
}