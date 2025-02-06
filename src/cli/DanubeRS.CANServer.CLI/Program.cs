using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CommandLine;
using DanubeRS.CANServer.Lib.BinaryLogs;
using DanubeRS.CANServer.Lib.Downloader;
using DanubeRS.CANServer.Lib.Downloader.Client;
using DanubeRS.CANServer.CLI;
using DanubeRS.CANServer.Lib.Pandas;
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

var parserResult =
    Parser.Default
        .ParseArguments<DownloadOptions, ParseOptions, DownloadAndParseOptions,
            ContinuousDownloadAndParseOptions>(args);

parserResult.MapResult(
    (DownloadOptions opts) => Download(opts).GetAwaiter().GetResult(),
    (ParseOptions opts) => Parse(opts).GetAwaiter().GetResult(),
    (DownloadAndParseOptions opts) => DownloadAndParse(opts).GetAwaiter().GetResult(),
    (ContinuousDownloadAndParseOptions opts) => ContinuousDownloadAndParse(opts).GetAwaiter().GetResult(),
    _ => 1
);
return;

async Task<int> ContinuousDownloadAndParse(ContinuousDownloadAndParseOptions opts,
    CancellationToken cancellationToken = default)
{
    var internalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    var url = new Uri(opts.Address);
    var clientFactory = new PandasClientFactory(url.Host, 1338, loggerFactory);
    var dbc = await BootstrapDatabase(opts);
    var influxClient = InfluxWriteClientFactory(opts);
    var shouldDownload = false;
    const int fileCountThreshold = 1;

    var client = await clientFactory.CreateAsync(message =>
    {
        var statFrame = message.Frames.SingleOrDefault(f => f.FrameId == 0x500);
        if (statFrame == null) return;
        if (!dbc.TryParseBinaryMessage(statFrame.FrameId, statFrame.FrameData, out var value, out var _)) return;
        var busARate = value.Signals.SingleOrDefault(s => s.SignalName == "CANServer_InterfaceARate");
        var busBRate = value.Signals.SingleOrDefault(s => s.SignalName == "CANServer_InterfaceBRate");
        if (busARate == null || busBRate == null) return;
        var busRate = (int)busARate.Value + (int)busBRate.Value;
        const int busActivityThreshold = 0;
        logger.LogDebug("Bus rate is {Rate}. Threshold is {Threshold}", busRate, busActivityThreshold);
        switch (busRate)
        {
            case > busActivityThreshold when shouldDownload || !internalCts.IsCancellationRequested:
            {
                // Don't download, as there is bus activity!
                logger.LogInformation(
                    "Bus has woken up ({BusRate}), cancel any active downloads and wait until its quiet again.",
                    busRate);
                shouldDownload = false;
                // Cancel any active downloading
                if (!internalCts.IsCancellationRequested)
                    internalCts.Cancel();
                break;
            }
            case <= busActivityThreshold when !shouldDownload:
            {
                logger.LogInformation(
                    "Bus activity ({BusRate}) is lower than threshold, start downloading and parsing files!", busRate);
                shouldDownload = true;
                if (internalCts.IsCancellationRequested) internalCts.TryReset();
                break;
            }
        }
    }, cancellationToken);

    var archiveDirectory = opts.ArchivePath?.Replace("~",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    var (queueWait, queueFile, queueComplete) = await BuildParsePipeline(dbc,
        (file, ct) => ArchiveDownloadedFile(file, archiveDirectory, ct), CancellationToken.None, influxClient, opts, opts);

    while (true)
    {
        if (cancellationToken.IsCancellationRequested) break;
        // There is no active logging, so lets go ahead and download!
        if (shouldDownload)
        {
            // TODO do a download file threshold check here?
            logger.LogInformation("Download is ready; warming up...");
            try
            {
                // Only download if there are more than one files (i.e: there is actually something to get!
                await DownloadInternal(opts, (files) =>
                    {
                        var fileCount = files?.Count();
                        if (fileCount <= fileCountThreshold)
                        {
                            logger.LogInformation("{FileCount} files are ready to download, but this is below the threshold ({FileCountThreshold})", fileCount, fileCountThreshold);
                            return false;
                        }
                        logger.LogInformation("{FileCount} files are ready to download; which is above the threshold ({FileCountThreshold}) running routine:", fileCount, fileCountThreshold);
                        return true;
                    }, queueFile, internalCts.Token,
                    forceRemove: true);
            }
            catch (TaskCanceledException)
            {
                // If there is a parent cancellation requested, then we must break completely...
                if (cancellationToken.IsCancellationRequested) break;
                // Otherwise, lets do another loop, as it's just the logging being re-enabled...
                logger.LogInformation("An active download was cancelled due to the bus waking up");
            }
        }

        internalCts.TryReset();
        logger.LogInformation("Download loop successfully ran; waiting {Watch}s before next loop", opts.Watch            );
        await Task.Delay(TimeSpan.FromSeconds(opts.Watch), cancellationToken);
    }

    queueComplete();
    await client.AliveHandle;
    await queueWait;
    return 0;
}

async Task<int> DownloadAndParse(DownloadAndParseOptions options)
{
    var influxClient = InfluxWriteClientFactory(options);
    var archiveDirectory = Path.Combine(options.OutputPath.Replace("~",
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "ARCHIVE");
    Directory.CreateDirectory(archiveDirectory);
    var dbc = await BootstrapDatabase(options);
    var (queueWait, queueFile, queueComplete) = await BuildParsePipeline(dbc,
        (s, ct) => ArchiveDownloadedFile(s, archiveDirectory, ct), CancellationToken.None, influxClient, options, options);
    await DownloadInternal(options, null, queueFile);
    queueComplete();
    await queueWait;
    return 0;
}

Task<bool> ArchiveDownloadedFile(FileInfo file, string? archiveDirectory,
    CancellationToken cancellationToken = default)
{
    if (archiveDirectory != null)
        file.MoveTo(Path.Combine(archiveDirectory, file.Name));
    return Task.FromResult(true);
}

async Task<(Task queueWait, Action<string> queueFile, Action onQueueComplete)> BuildParsePipeline(Database dbc,
    Func<FileInfo, CancellationToken, Task<bool>> onSuccessfulParse, CancellationToken cancellationToken,
    WriteApiAsync influxClient, IUploadOptions options, IParseOptions parseOptions)
{
    var inputFile = new BufferBlock<string>();
    var writeFile = new ActionBlock<string>(async (s) =>
    {
        var fileInfo = new FileInfo(s);
        await ParseFileInternal(fileInfo, dbc, influxClient, options, parseOptions);
        await onSuccessfulParse(fileInfo, cancellationToken);
    }, new ExecutionDataflowBlockOptions
    {
        MaxDegreeOfParallelism = 1,
    });
    inputFile.LinkTo(writeFile, new DataflowLinkOptions { PropagateCompletion = true });
    return (writeFile.Completion, s => inputFile.Post(s), inputFile.Complete);
}

async Task<int> Download(DownloadOptions options)
{
    return await DownloadInternal(options);
}

async Task<int> DownloadInternal(IDownloadOptions downloadOptions,
    Func<IEnumerable<LogFileRecord>, bool>? shouldRunTest = null, Action<string>? onFileDownloaded = null,
    CancellationToken cancellationToken = default, bool forceRemove = false)
{
    var downloader = new Downloader(downloadOptions.Address, loggerFactory.CreateLogger<Downloader>());
    await downloader.DownloadAllFiles(LogFileType.Interval, downloadOptions.Remove || forceRemove, true,
        downloadOptions.OutputPath,
        cancellationToken, onFileDownloaded, shouldRunTest);
    return 0;
}

async Task<int> Parse(ParseOptions options)
{
    var influxClient = InfluxWriteClientFactory(options);
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
            await ParseFileInternal(file, database, influxClient, options, options);
            if (archiveDirectory != null)
                file.MoveTo(Path.Combine(archiveDirectory, Path.Combine(archiveDirectory, file.Name)));
        }

        if (options.Watch <= 0) return 0;

        logger.LogInformation("Parsing is watching, delay {Seconds} before next loop...", options.Watch);
        await Task.Delay(TimeSpan.FromSeconds(options.Watch));
    }

    return 0;
}

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
    Database database, WriteApiAsync writeApiAsync, IUploadOptions uploadOptions, IParseOptions parseOptions)
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
            if ((parseOptions.Signals?.Any() ?? false) && !parseOptions.Signals.Contains(frame.FrameId)) continue;
            logger.LogDebug("Successfully Parsed message #{MessageNumber} @ {MessageTime} (ID:{FrameId})", frames,
                frame.FrameTime, frame.FrameId);
            logger.LogTrace("{MessageId} {MessageName} {Signals}", value.MessageId, value.MessageName,
                value.Signals.Select(s => $"{s.SignalName}"));

            pointData.AddRange(value.Signals
                .Select(signal => new
                    { signal, signalDfn = defn.Signals.First(s => s.Signal.Name == signal.SignalName) })
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
            await writeApiAsync.WritePointsAsync(pointData, uploadOptions.InfluxBucket, uploadOptions.InfluxOrg);
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

WriteApiAsync InfluxWriteClientFactory(IUploadOptions opts)
{
    var influxClient = new InfluxDBClient(opts.InfluxAddress, opts.InfluxToken);
    if (!opts.InfluxCompress)
        influxClient.DisableGzip();
    return influxClient.GetWriteApiAsync();
}