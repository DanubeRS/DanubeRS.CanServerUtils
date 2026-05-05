using System.Formats.Tar;
using System.IO.Compression;
using DanubeRS.CANServer.CLI.Commands.Upload;
using Microsoft.Extensions.Logging.Abstractions;
using ZstdSharp;

namespace DanubeRS.CANServer.CLI.Tests;

public class ArchiveCompressorTests : IDisposable
{
    private readonly string _tempDir;

    public ArchiveCompressorTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"archive-compressor-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    [Fact]
    public void CompressDateDirectory_CreatesTarZst()
    {
        var dateKey = "2025-01-30";
        var dateDir = Path.Combine(_tempDir, dateKey);
        Directory.CreateDirectory(dateDir);

        File.WriteAllText(Path.Combine(dateDir, "2025-01-30T14-30-00_0.log"), "test data 0");
        File.WriteAllText(Path.Combine(dateDir, "2025-01-30T14-30-00_1.log"), "test data 1");

        ArchiveCompressor.CompressDateDirectory(dateKey, dateDir, _tempDir, NullLogger.Instance);

        var archivePath = Path.Combine(_tempDir, $"{dateKey}.tar.zst");
        File.Exists(archivePath).Should().BeTrue();
        Directory.Exists(dateDir).Should().BeFalse();
    }

    [Fact]
    public void CompressDateDirectory_IncrementOnCollision()
    {
        var dateKey = "2025-01-30";
        var dateDir = Path.Combine(_tempDir, dateKey);
        Directory.CreateDirectory(dateDir);
        File.WriteAllText(Path.Combine(dateDir, "2025-01-30T14-30-00_0.log"), "test data");

        var existingArchive = Path.Combine(_tempDir, $"{dateKey}.tar.zst");
        File.WriteAllText(existingArchive, "existing archive");

        ArchiveCompressor.CompressDateDirectory(dateKey, dateDir, _tempDir, NullLogger.Instance);

        var incrementPath = Path.Combine(_tempDir, $"{dateKey}_1.tar.zst");
        File.Exists(incrementPath).Should().BeTrue();
        File.Exists(existingArchive).Should().BeTrue();
    }

    [Fact]
    public void CompressDateDirectory_ArchiveIsDecompressible()
    {
        var dateKey = "2025-01-30";
        var dateDir = Path.Combine(_tempDir, dateKey);
        Directory.CreateDirectory(dateDir);

        File.WriteAllText(Path.Combine(dateDir, "test.log"), "hello world");

        ArchiveCompressor.CompressDateDirectory(dateKey, dateDir, _tempDir, NullLogger.Instance);

        var archivePath = Path.Combine(_tempDir, $"{dateKey}.tar.zst");

        using var fileStream = File.OpenRead(archivePath);
        using var decompressionStream = new DecompressionStream(fileStream);
        using var tarReader = new TarReader(decompressionStream);

        var entries = new List<string>();
        TarEntry? entry;
        while ((entry = tarReader.GetNextEntry()) != null)
        {
            entries.Add(entry.Name);
        }

        entries.Should().Contain("test.log");
    }

    [Fact]
    public void CompressDateDirectory_PreservesDateDirectoryOnError()
    {
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
            return;

        var dateKey = "2025-01-30";
        var dateDir = Path.Combine(_tempDir, dateKey);
        Directory.CreateDirectory(dateDir);
        File.WriteAllText(Path.Combine(dateDir, "test.log"), "data");

        var readOnlyDir = Path.Combine(_tempDir, "readonly");
        Directory.CreateDirectory(readOnlyDir);
#pragma warning disable CA1416
        File.SetUnixFileMode(readOnlyDir, UnixFileMode.UserRead | UnixFileMode.UserExecute);
#pragma warning restore CA1416

        try
        {
            ArchiveCompressor.CompressDateDirectory(dateKey, dateDir, readOnlyDir, NullLogger.Instance);
        }
        finally
        {
#pragma warning disable CA1416
            File.SetUnixFileMode(readOnlyDir, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
#pragma warning restore CA1416
        }

        Directory.Exists(dateDir).Should().BeTrue();
        File.Exists(Path.Combine(dateDir, "test.log")).Should().BeTrue();
    }
}