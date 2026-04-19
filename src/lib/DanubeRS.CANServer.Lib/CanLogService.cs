using DanubeRS.CANServer.Lib.BinaryLogs;
using Microsoft.Extensions.Logging;

namespace DanubeRS.CANServer.Lib;

public class CanLogService
{
    private readonly ILogger<CanLogService> _logger;
    private List<LogReaderFrame> _frames = [];
    private byte[]? _rawData;

    public CanLogService(ILogger<CanLogService> logger)
    {
        _logger = logger;
    }

    public bool IsLoaded => _frames.Count > 0;

    public int FrameCount => _frames.Count;

    public async Task LoadLogAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        _rawData = ms.ToArray();

        using var dataStream = new MemoryStream(_rawData);
        var reader = await Reader.Open(dataStream, _logger);
        if (reader == null)
            throw new InvalidOperationException("Failed to open log file. Invalid format or corrupted data.");

        var frames = new List<LogReaderFrame>();
        LogReaderFrame? frame;
        while ((frame = reader.GetNextFrame()) != null)
        {
            frames.Add(frame);
        }

        _frames = frames;
        reader.Dispose();
        _logger.LogInformation("Loaded {FrameCount} frames from log file", _frames.Count);
    }

    public void LoadLog(byte[] data)
    {
        using var stream = new MemoryStream(data);
        LoadLogAsync(stream).GetAwaiter().GetResult();
    }

    public IReadOnlyList<LogReaderFrame> GetFrames(int skip, int take, uint? frameIdFilter = null)
    {
        EnsureLoaded();

        var query = _frames.AsEnumerable();
        if (frameIdFilter.HasValue)
            query = query.Where(f => f.FrameId == frameIdFilter.Value);

        return query.Skip(skip).Take(take).ToList();
    }

    public LogReaderFrame GetFrame(int index)
    {
        EnsureLoaded();
        if (index < 0 || index >= _frames.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"Frame index {index} is out of range (0-{_frames.Count - 1})");

        return _frames[index];
    }

    public IEnumerable<uint> GetDistinctFrameIds()
    {
        EnsureLoaded();
        return _frames.Select(f => f.FrameId).Distinct().Order();
    }

    public void Clear()
    {
        _frames = [];
        _rawData = null;
    }

    private void EnsureLoaded()
    {
        if (_frames.Count == 0)
            throw new InvalidOperationException("No log file loaded. Call LoadLog first.");
    }
}