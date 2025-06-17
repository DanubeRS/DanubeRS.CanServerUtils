using System.Security.Cryptography;
using System.Text.RegularExpressions;
using FFMpegCore;

namespace DanubeRS.CANServer.CLI.VideoJoiner;

public enum ClipType
{
    RecentClips,
    SavedClips,
    SentryClips,
}

public enum CameraAngle
{
    Front,
    Back,
    LeftRepeater,
    RightRepeater,
}

public record ScanRequest(DirectoryInfo Directory, ClipType[]? ClipTypes = null);

public record ScanProgress();

public record ScanFile(CameraAngle Angle, FileInfo File);

public record ScanResult(
    ClipType Type,
    DateTimeOffset Start,
    DateTimeOffset End,
    HashSet<(DateTimeOffset timestamp, ScanFile[] files)> Files,
    FileInfo? MetadataFile = null);

public record ScanResults(ScanResult[] Results);

public static partial class Scanner
{
    public static Task<ScanResults> Scan(ScanRequest request, IProgress<ScanProgress> progress,
        CancellationToken cancellationToken = default)
    {
        var clipTypes = request.ClipTypes ?? [ClipType.SavedClips, ClipType.SentryClips];
        var filterAfter =
            DateTimeOffset.MinValue;
        // DateTimeOffset.Parse("2025-03-16T12:00:00+10:00");
        var filterBefore =
            DateTimeOffset.MaxValue;
        // DateTimeOffset.Parse("2025-03-17T00:00:00+10:00");
        var dirs = request.Directory.GetDirectories();
        var clips = dirs.SelectMany(d => d switch
        {
            not null when d.Name == Enum.GetName(ClipType.RecentClips) && clipTypes.Contains(ClipType.RecentClips) =>
                GetRecentClips(request, d),
            not null when d.Name == Enum.GetName(ClipType.SavedClips) && clipTypes.Contains(ClipType.SavedClips) =>
                GetEventClips(request, d, ClipType.SavedClips),
            not null when d.Name == Enum.GetName(ClipType.SentryClips) && clipTypes.Contains(ClipType.SentryClips) =>
                GetEventClips(request, d, ClipType.SentryClips),
            _ => []
        }).ToArray();
        return Task.FromResult(
            new ScanResults(clips.Where(e => e.Start > filterAfter && e.End < filterBefore).ToArray()));
    }

    private static IEnumerable<ScanResult> GetRecentClips(ScanRequest request, DirectoryInfo dir)
    {
        var files = dir.GetDirectories().SelectMany(d => d.GetFiles()).Select(GetClipFileDetails).Where(f => f.success)
            .Select(f => f.result!.Value)
            // .Where(f => f.timestamp > filterAfter && f.timestamp < filterBefore)
            .OrderBy(g => g.timestamp);

        var events = GetEvents(files, 120).ToArray().Select(e =>
        {
            var ordered = e.OrderBy(e => e.timestamp).ToArray();
            return new ScanResult(ClipType.RecentClips, ordered.First().timestamp,
                ordered.Last().timestamp + FFProbe.Analyse(ordered.Last().f.FullName).Duration,
                ordered.GroupBy(o => o.timestamp)
                    .Select(o => (o.Key, o.Select(f => new ScanFile(f.cameraAngle, f.f)).ToArray())).ToHashSet());
        }).ToArray();
        return events;
    }

    private static IEnumerable<ScanResult> GetEventClips(ScanRequest request, DirectoryInfo dir, ClipType clipType)
    {
        foreach (var eventDir in dir.GetDirectories().OrderBy(d => d.Name))
        {
            var clipFileGroups = eventDir.GetFiles().Select(GetClipFileDetails).Where(f => f.success)
                .Select(f => f.result!.Value).OrderBy(c => c.timestamp).GroupBy(c => c.timestamp).OrderBy(g => g.Key)
                .ToArray();

            if (clipFileGroups.Length == 0) continue;

            var firstClipTimestamp = clipFileGroups.First().Key;
            var lastClips = clipFileGroups.Last();
            var lastClipDurations = lastClips.Select(c => FFProbe.Analyse(c.f.FullName).Duration);
            var files = clipFileGroups.Select(o => (o.Key, o.Select(f => new ScanFile(f.cameraAngle, f.f)).ToArray()))
                .ToHashSet();

            yield return new ScanResult(clipType, firstClipTimestamp, lastClips.Key + lastClipDurations.Max(), files,
                eventDir.GetFiles("*.json").SingleOrDefault());
        }
    }

    private static IEnumerable<(FileInfo f, CameraAngle cameraAngle, DateTimeOffset timestamp)[]> GetEvents(
        IOrderedEnumerable<(FileInfo f, CameraAngle cameraAngle, DateTimeOffset timestamp)> files, int chunkGapSeconds)
    {
        var list = new List<(FileInfo, CameraAngle angle, DateTimeOffset timestamp)>();
        using var enumerator = files.GetEnumerator();
        var lastFileTime = DateTimeOffset.MinValue;
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.timestamp - TimeSpan.FromSeconds(chunkGapSeconds) > lastFileTime)
            {
                var chunk = list.ToArray();
                if (chunk.Length != 0)
                {
                    list.Clear();
                    yield return chunk;
                }
            }

            // var (f, cameraAngle, timestamp) = enumerator.Current;

            // list.Add((f, cameraAngle, timestamp, ));
            list.Add(enumerator.Current);
            lastFileTime = enumerator.Current.timestamp;
        }

        yield return list.ToArray();
    }

    private static (bool success, (FileInfo f, CameraAngle cameraAngle, DateTimeOffset timestamp)? result)
        GetClipFileDetails(FileInfo fileInfo)
    {
        var match = ClipRegex().Match(fileInfo.Name);
        if (!match.Success) return (false, null);
        var date = new DateOnly(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value),
            int.Parse(match.Groups["day"].Value));
        var time = new TimeOnly(int.Parse(match.Groups["hour"].Value), int.Parse(match.Groups["minute"].Value),
            int.Parse(match.Groups["second"].Value));
        var dateTime = new DateTimeOffset(date, time, TimeSpan.FromHours(10));
        CameraAngle? angle = match.Groups["cam"].Value switch
        {
            "front" => CameraAngle.Front,
            "back" => CameraAngle.Back,
            "left_repeater" => CameraAngle.LeftRepeater,
            "right_repeater" => CameraAngle.RightRepeater,
            _ => null
        };

        if (angle == null) return (false, null);

        return (true, (fileInfo, angle.Value, dateTime));
    }

    [GeneratedRegex(
        "^(?'year'\\d{4})-(?'month'\\d{2})-(?'day'\\d{2})_(?'hour'\\d{2})-(?'minute'\\d{2})-(?'second'\\d{2})-(?'cam'[\\w]+)\\.mp4")]
    private static partial Regex ClipRegex();

    [GeneratedRegex(
        "^(?'year'\\d{4})-(?'month'\\d{2})-(?'day'\\d{2})_(?'hour'\\d{2})-(?'minute'\\d{2})-(?'second'\\d{2})\\.mp4")]
    private static partial Regex EventDirRegex();
}