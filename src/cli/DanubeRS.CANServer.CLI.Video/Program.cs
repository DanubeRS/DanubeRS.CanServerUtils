// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.Sockets;
using SkiaSharp;

const int totalFrames = 1000;
var socketPath = Path.Combine(Path.GetTempPath(), "sock.sock");
var inputArgs = $"-framerate 25 -y -f rawvideo -pix_fmt rgb32 -video_size 1920x1080 -listen 1 -i unix:{socketPath}";
var outputArgs = "-vcodec libx264 -crf 15 -pix_fmt yuv420p -preset ultrafast -r 25 out.mp4";

var process = new Process
{
    StartInfo =
    {
        FileName = "ffmpeg",
        Arguments = $"{inputArgs} {outputArgs}",
        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardInput = true
    }
};


SKImage DrawFrame(int i, int total)
{
    var info = new SKImageInfo(1920, 1080);
    using var surface = SKSurface.Create(info);
    var canvas = surface.Canvas;

    using var paint = new SKPaint();
    paint.Color = SKColors.White;
    paint.Style = SKPaintStyle.Fill;
    paint.IsAntialias = true;

    const int radius = 30;
    var coord = new SKPoint(1920 / (float)total * i, 1080 / (float)total * i);
    canvas.DrawCircle(coord, radius, paint);
    return surface.Snapshot();
}

// using var fs = File.Open("/tmp/test", FileMode.Open, FileAccess.Write);
// delete file if it exists
if (File.Exists(socketPath))
    File.Delete(socketPath);
using var sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
process.Start();

await Task.Delay(1000);

var endpoint = new UnixDomainSocketEndPoint(socketPath);
await sock.ConnectAsync(endpoint);

var frames = 0;
while (frames < totalFrames)
{
    using var frame = DrawFrame(frames, totalFrames);
    using var pixmap = SKBitmap.FromImage(frame);
    sock.Send(pixmap.GetPixelSpan());
    frames++;
}

sock.Disconnect(false);

await process.WaitForExitAsync();

File.Delete(socketPath);