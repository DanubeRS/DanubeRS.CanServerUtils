using System.Collections;
using System.Diagnostics;
using System.Text;
using FFMpegCore;
using FFMpegCore.Arguments;

namespace DanubeRS.CANServer.CLI.VideoJoiner;

public class VideoBuilder
{
    private readonly string _outDir;
    private readonly string _cacheDir;

    public VideoBuilder(string outDir, string cacheDir)
    {
        _outDir = outDir;
        _cacheDir = cacheDir;
    }

    public async Task Build(ScanResult result, CameraAngle angle)
    {
        var files = result.Files.Select(rf =>
                (rf.timestamp, rf.files.SingleOrDefault(f => f.Angle == angle)))
            .Where(f => f.Item2 != null)
            .OrderBy(f => f.timestamp)
            .ToArray();

        if (files.Length == 0) return;

        var firstClip = files.First();
        var lastClip = files.Last();
        var start = firstClip.timestamp;
        var end = lastClip.timestamp;

        var finalName =
            $"{result.Type:G}-{start:yyyyMMdd_hhmmss}-{end:yyyyMMdd_hhmmss}-{Enum.GetName(angle)!.ToLowerInvariant()}.mp4"
                .Replace(':', '_');
        var finalPath = Path.Combine(_outDir, finalName);
        var tempPath = Path.Combine(_cacheDir, $"_{Guid.NewGuid()}.tmp.mp4");

        if (File.Exists(finalPath))
        {
            return;
        }


        var duration = lastClip.timestamp - firstClip.timestamp +
                       (await FFProbe.AnalyseAsync(lastClip.Item2.File.FullName)).Duration;

        var argsString = new StringBuilder("-y ");
        // Inputs
        argsString.Append(string.Join("", files.Select(c => $"-i {c.Item2!.File.FullName} ")));
        // Filter
        var filterString = new StringBuilder();
        // Filter setpts
        filterString.Append(string.Concat(files.Select((f, i) =>
            $"[{i}:v]setpts=PTS-STARTPTS+{(f.timestamp - firstClip.timestamp).TotalSeconds}[v{i}];")));
        // Filter concat
        filterString.Append(
            $"{string.Concat(files.Select((_, i) => $"[v{i}]"))}concat=n={files.Length}:v=1[out]");
        argsString.Append($"-filter_complex \"{filterString.ToString()}\" ");
        argsString.Append(
            $"-map [out] -t {duration.TotalSeconds} -c:v libx265 -crf 18 -pix_fmt yuv420p -preset ultrafast {tempPath}");

        var exitCode = 0;
        var attempts = 0;
        do
        {
            attempts++;
            var process = Process.Start("ffmpeg", argsString.ToString());
            process.PriorityClass = ProcessPriorityClass.BelowNormal;
            await process.WaitForExitAsync();
            exitCode = process.ExitCode;
        } while (exitCode != 0 && attempts <= 3);

        try
        {
            File.Copy(tempPath, finalPath, true);
            try
            {
                File.Delete(tempPath);
            }
            catch
            {
                // ignored
            }
        }
        catch
        {
            // ignored
        }
    }
}