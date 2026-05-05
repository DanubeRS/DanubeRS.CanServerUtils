using DanubeRS.CANServer.CLI.Commands.Upload;

namespace DanubeRS.CANServer.CLI.Tests;

public class ArchiveNamingTests : IDisposable
{
    private readonly string _tempDir;

    public ArchiveNamingTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private FileInfo CreateTempFile(string name)
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, "");
        return new FileInfo(path);
    }

    private static List<string> RunArchiveNaming(List<FileProcessingResult> results)
    {
        var timestampCounts = new Dictionary<string, int>();
        var archiveNames = new List<string>();

        foreach (var result in results)
        {
            var timestamp = result.FirstTimestamp ?? result.File.CreationTime;
            var timestampKey = timestamp.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");

            if (!timestampCounts.TryGetValue(timestampKey, out int increment))
            {
                increment = 0;
            }

            if (increment > 9)
            {
                continue;
            }

            var archiveFileName = $"{timestampKey}_{increment}.log";
            archiveNames.Add(archiveFileName);

            timestampCounts[timestampKey] = increment + 1;
        }

        return archiveNames;
    }

    [Fact]
    public void ArchiveNaming_TwoFilesSameTimestamp_GetsIncrementingSuffixes()
    {
        var ts = new DateTimeOffset(2025, 3, 15, 10, 30, 0, TimeSpan.Zero);
        var results = new List<FileProcessingResult>
        {
            new(CreateTempFile("file1.log"), ts),
            new(CreateTempFile("file2.log"), ts),
            new(CreateTempFile("file3.log"), ts),
        };

        var archiveNames = RunArchiveNaming(results);
        var key = ts.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");

        archiveNames.Should().Equal($"{key}_0.log", $"{key}_1.log", $"{key}_2.log");
    }

    [Fact]
    public void ArchiveNaming_DifferentTimestamps_GetIndependentSuffixes()
    {
        var ts1 = new DateTimeOffset(2025, 3, 15, 10, 30, 0, TimeSpan.Zero);
        var ts2 = new DateTimeOffset(2025, 3, 15, 11, 45, 0, TimeSpan.Zero);
        var results = new List<FileProcessingResult>
        {
            new(CreateTempFile("file1.log"), ts1),
            new(CreateTempFile("file2.log"), ts2),
        };

        var archiveNames = RunArchiveNaming(results);
        var key1 = ts1.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");
        var key2 = ts2.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");

        archiveNames.Should().Equal($"{key1}_0.log", $"{key2}_0.log");
    }

    [Fact]
    public void ArchiveNaming_IncrementBeyond9_StopsAt9()
    {
        var ts = new DateTimeOffset(2025, 3, 15, 10, 30, 0, TimeSpan.Zero);
        var results = new List<FileProcessingResult>();
        for (int i = 0; i <= 10; i++)
        {
            results.Add(new(CreateTempFile($"file{i}.log"), ts));
        }

        var archiveNames = RunArchiveNaming(results);
        var key = ts.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");

        archiveNames.Count.Should().Be(10);
        archiveNames[9].Should().Be($"{key}_9.log");
    }

    [Fact]
    public void ArchiveNaming_FirstTimestampNull_FallsBackToFileCreationTime()
    {
        var results = new List<FileProcessingResult>
        {
            new(CreateTempFile("file1.log"), null),
        };

        var archiveNames = RunArchiveNaming(results);

        archiveNames.Should().HaveCount(1);
        var creationTime = results[0].File.CreationTime;
        var expectedKey = creationTime.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");
        archiveNames[0].Should().Be($"{expectedKey}_0.log");
    }
}