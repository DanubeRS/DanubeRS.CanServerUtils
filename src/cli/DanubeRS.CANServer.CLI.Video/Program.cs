// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.Sockets;
using DanubeRS.CANServer.UI.SkiaElements.Controls;
using InfluxDB.Client;
using NodaTime;
using SkiaSharp;
using File = System.IO.File;

const int rate = 25;
var queryChunkDuration = TimeSpan.FromMinutes(1);

var socketPath = Path.Combine(Path.GetTempPath(), "sock.sock");

var inputArgs =
    $"-framerate {rate} -y -f rawvideo -pix_fmt rgb32 -video_size 1920x1080 -listen 1 -i unix:{socketPath} -i clip.mp4";
var filterArgs = $"[1]scale=iw/2:ih/2 [pip];[0][pip]overlay=main_w-overlay_w-10:main_h-overlay_h-50";
var outputArgs = $"-vcodec libx264 -crf 15 -pix_fmt yuv420p -preset ultrafast -r {rate} out.mp4";

var process = new Process
{
    StartInfo =
    {
        FileName = "ffmpeg",
        Arguments = $"{inputArgs} -filter_complex \"{filterArgs}\" {outputArgs}",
        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardInput = true
    }
};

var start = DateTimeOffset.Parse("2025-03-02 06:05:16");
var end = DateTimeOffset.Parse("2025-03-02 06:06:16");
var duration = end - start;


var speedGauge = new Gauge("Speed", 0, 100);
var powerGauge = new VerticalGauge(-70, 380);


var info = new SKImageInfo(1920, 1080);
using var surface = SKSurface.Create(info);
var canvas = surface.Canvas;

SKImage DrawFrame(double speed, double power, DateTimeOffset time)
{
    canvas.Clear(SKColors.Black);
    speedGauge.Render(canvas, new GaugeValue((decimal)speed), 300, 200, 100);
    powerGauge.Render(canvas, new VerticalGaugeValue((decimal)power / 1000), 100, 1080);
    canvas.DrawText(time.ToString("O"), 100, 1060, SKTextAlign.Left,
        new SKFont(SKTypeface.FromFamilyName("monospace"), 24), new SKPaint { Color = SKColors.White });

    return surface.Snapshot();
}

if (File.Exists(socketPath))
    File.Delete(socketPath);

using var sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
process.Start();

await Task.Delay(1000);

var endpoint = new UnixDomainSocketEndPoint(socketPath);
await sock.ConnectAsync(endpoint);

var client = new InfluxDBClient("https://influx.internal.danubers.com",
    "AIiYRM3WGnIq6_fERvrqT_Pydm0hB21kePpgJHmEtCn0HV3AyoX0Au3EBfONGMUs86ym4mpztjZV9pDa7j_2oA==");

client.EnableGzip();

var query = client.GetQueryApi();

var currentTime = start;
while (currentTime < end)
{
    var queryDuration =
        TimeSpan.FromSeconds(Math.Min((end - currentTime).TotalSeconds, queryChunkDuration.TotalSeconds));
    var queryStart = currentTime;
    var queryStop = currentTime + queryDuration;
    var data = await query.QueryAsync(@$"
speed = from(bucket: ""tesla"")
  |> range(start: {queryStart:O}, stop: {queryStop:O})
  |> filter(fn: (r) => r[""_measurement""] == ""CAN"")
  |> filter(fn: (r) => r[""_field""] == ""ESP_vehicleSpeed"")
  |> aggregateWindow(every: 40ms, fn: last, createEmpty: false)
  |> yield(name: ""speed"")

voltage = from(bucket: ""tesla"")
  |> range(start: {queryStart:O}, stop: {queryStop:O})
  |> filter(fn: (r) => r[""_field""] == ""BattVoltage132"")
  |> aggregateWindow(every: 40ms, fn: last, createEmpty: false)

current = from(bucket: ""tesla"")
  |> range(start: {queryStart:O}, stop: {queryStop:O})
  |> filter(fn: (r) => r[""_field""] == ""SmoothBattCurrent132"")
  |> aggregateWindow(every: 40ms, fn: last, createEmpty: false)

power = join(tables: {{voltage: voltage, current: current}}, on: [""_time""])
  |> map(fn: (r) => ({{_time: r._time, _value: r._value_voltage * r._value_current, _field: ""power""}}))
  |> yield(name: ""power"")
"
        , "danubers");
    var keyedData = data.ToDictionary(table => table.Records.First().Values["_field"], table => table.Records
        .Select(r =>
        {
            var time = r.Values["_time"] as Instant? ?? default;
            var value = r.Values["_value"] as double? ?? 0;
            return new { time = time.ToDateTimeOffset(), value };
            ;
        }).OrderBy(r => r.time));

    var frames = queryDuration.TotalSeconds * rate;
    var frame = 0;
    using var speedIterator = keyedData["ESP_vehicleSpeed"].GetEnumerator();
    using var powerIterator = keyedData["power"].GetEnumerator();
    speedIterator.MoveNext();
    powerIterator.MoveNext();
    var currentSpeed = speedIterator.Current;
    var currentPower = powerIterator.Current;
    while (frame < frames)
    {
        var frameTime = currentTime + TimeSpan.FromMilliseconds((long)(1 / (decimal)rate * frame * 1000));
        while (currentSpeed.time < frameTime)
        {
            currentSpeed = speedIterator.Current;
            speedIterator.MoveNext();
        }

        while (currentPower.time < frameTime)
        {
            currentPower = powerIterator.Current;
            powerIterator.MoveNext();
        }

        using var frameImage = DrawFrame(currentSpeed.value, currentPower.value, currentSpeed.time);
        using var framePixmap = SKBitmap.FromImage(frameImage);
        sock.Send(framePixmap.GetPixelSpan());
        frame++;
    }

    currentTime += queryDuration;
}

sock.Disconnect(false);

await process.WaitForExitAsync();

File.Delete(socketPath);