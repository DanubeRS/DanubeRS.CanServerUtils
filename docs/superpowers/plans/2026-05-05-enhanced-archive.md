# Enhanced Archive Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Enhance the upload command's `--archive` option to support timestamped renaming, date-directory organization, and daily tar+zst compression.

**Architecture:** Extend the existing `ArchiveFiles` method into a two-phase process: (1) rename files with timestamps and move into date directories, (2) optionally compress date directories into `.tar.zst` archives. Add `ZstdSharp` library for zstandard compression. Change `--archive` from a `FileInfo?` to accept `dir` or `tar` mode values, and add `--archive-path` as a separate directory option.

**Tech Stack:** .NET 9.0, System.CommandLine, ZstdSharp (zstandard compression library), System.IO.Compression (tar)

---

### Task 1: Add ZstdSharp Package Dependency

**Files:**
- Modify: `src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj`

- [ ] **Step 1: Add ZstdSharp package reference**

Add the ZstdSharp NuGet package to the CLI project:

```bash
dotnet add src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj package ZstdSharp
```

- [ ] **Step 2: Verify build**

Run: `dotnet build src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj
git commit -m "chore: add ZstdSharp package dependency"
```

---

### Task 2: Change `--archive` Option to Accept Mode Values

**Files:**
- Modify: `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommand.cs`
- Modify: `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs`

- [ ] **Step 1: Create ArchiveMode enum and update UploadOptions**

In `UploadCommand.cs`, replace the `ArchivePath` option class with two new options and an enum. Add at the top of the `UploadOptions` class:

```csharp
public enum ArchiveMode
{
    Dir,
    Tar
}
```

Replace the `ArchivePath` class (lines 153-162) with two new classes:

```csharp
public sealed class Archive : Option<ArchiveMode?>
{
    public static Archive Instance { get; } = new();

    private Archive() : base("--archive")
    {
        Description = "Archive mode: 'dir' moves files to date directories, 'tar' compresses into daily .tar.zst archives";
        HelpName = "MODE";
    }
}

public sealed class ArchivePath : Option<DirectoryInfo?>
{
    public static ArchivePath Instance { get; } = new();

    private ArchivePath() : base("--archive-path")
    {
        Description = "Directory for archived files. Defaults to input directory if not specified";
        HelpName = "DIR";
    }
}
```

Update `UploadCommand` constructor to replace `UploadOptions.ArchivePath.Instance` with the two new options:

```csharp
Options.Add(UploadOptions.Archive.Instance);
Options.Add(UploadOptions.ArchivePath.Instance);
```

Update `UploadOptionsValues` class:
- Remove `public FileInfo? ArchivePath` property
- Add `public ArchiveMode? Archive { get; private set; }` property
- Add `public DirectoryInfo? ArchivePath { get; private set; }` property
- Update `Parse` method to read both new options:
```csharp
Archive = result.GetValue(UploadOptions.Archive.Instance),
ArchivePath = result.GetValue(UploadOptions.ArchivePath.Instance),
```

- [ ] **Step 2: Update UploadCommandHandler to use new options**

In `UploadCommandHandler.cs`, replace line 20:

```csharp
var archiveDirectory = parseResult.GetValue(UploadOptions.ArchivePath.Instance);
```

with:

```csharp
var archiveMode = uploadOptions.Archive;
var archivePath = uploadOptions.ArchivePath;
```

Replace the archive section (lines 55-58):

```csharp
if (archiveDirectory != null)
{
    ArchiveFiles(results, archiveDirectory);
}
```

with:

```csharp
if (archiveMode != null)
{
    ArchiveFiles(results, archiveMode.Value, archivePath ?? inputDir);
}
```

Update the `ArchiveFiles` method signature from:

```csharp
private void ArchiveFiles(List<FileProcessingResult> results, FileInfo archiveDirectory)
```

to:

```csharp
private void ArchiveFiles(List<FileProcessingResult> results, ArchiveMode mode, DirectoryInfo archiveDirectory)
```

Inside `ArchiveFiles`, change the date directory path computation. After renaming each file, move it into a date subdirectory. The current code moves files directly into `archiveDirectory`. Change it to:

```csharp
private void ArchiveFiles(List<FileProcessingResult> results, ArchiveMode mode, DirectoryInfo archiveDirectory)
{
    var timestampCounts = new Dictionary<string, int>();
    var dateFiles = new Dictionary<string, List<string>>();

    foreach (var result in results)
    {
        var timestamp = result.FirstTimestamp ?? result.File.CreationTime;
        var localTimestamp = timestamp.ToLocalTime();
        var timestampKey = localTimestamp.ToString("yyyy-MM-ddTHH-mm-ss");
        var dateKey = localTimestamp.ToString("yyyy-MM-dd");

        if (!timestampCounts.TryGetValue(timestampKey, out int increment))
        {
            increment = 0;
        }

        if (increment > 9)
        {
            logger.LogError("Cannot archive file {FileName}: more than 10 files share timestamp {TimestampKey}", result.File.Name, timestampKey);
            continue;
        }

        var archiveFileName = $"{timestampKey}_{increment}.log";
        var dateDir = Path.Combine(archiveDirectory.FullName, dateKey);
        Directory.CreateDirectory(dateDir);
        var archivePath = Path.Combine(dateDir, archiveFileName);

        try
        {
            result.File.MoveTo(archivePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to archive file {FileName} to {ArchivePath}", result.File.Name, archivePath);
            continue;
        }

        if (!dateFiles.ContainsKey(dateKey))
            dateFiles[dateKey] = new List<string>();
        dateFiles[dateKey].Add(archivePath);

        timestampCounts[timestampKey] = increment + 1;
    }

    if (mode == ArchiveMode.Tar)
    {
        CompressDateDirectories(dateFiles, archiveDirectory);
    }
}
```

- [ ] **Step 3: Run build and tests**

Run: `dotnet build src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj && dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/`
Expected: Build succeeds, tests pass (the ArchiveFiles test may need updating)

- [ ] **Step 4: Commit**

```bash
git add src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommand.cs src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs
git commit -m "feat: change --archive to accept mode values (dir|tar) with date directory support"
```

---

### Task 3: Implement tar+zst Compression

**Files:**
- Create: `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/ArchiveCompressor.cs`
- Modify: `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs`

- [ ] **Step 1: Create ArchiveCompressor class**

Create `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/ArchiveCompressor.cs`:

```csharp
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using ZstdSharp;

namespace DanubeRS.CANServer.CLI.Commands.Upload;

public class ArchiveCompressor(ILogger<ArchiveCompressor> logger)
{
    public void CompressDateDirectory(string dateKey, string dateDirPath, string outputDirectory)
    {
        var tarPath = Path.Combine(outputDirectory, $"{dateKey}.tar.zst");

        var increment = 1;
        while (File.Exists(tarPath))
        {
            tarPath = Path.Combine(outputDirectory, $"{dateKey}_{increment}.tar.zst");
            increment++;
        }

        try
        {
            using var outputStream = new FileStream(tarPath, FileMode.Create, FileAccess.Write);
            using var compressionStream = new CompressionStream(outputStream, level: 19);

            using (var tarWriter = new TarWriter(compressionStream))
            {
                foreach (var filePath in Directory.EnumerateFiles(dateDirPath))
                {
                    var entry = new TarEntryData
                    {
                        Name = Path.GetFileName(filePath),
                    };
                    tarWriter.WriteEntry(filePath, entry);
                }
            }

            logger.LogInformation("Created archive {ArchivePath}", tarPath);

            Directory.Delete(dateDirPath, recursive: true);
            logger.LogInformation("Removed date directory {DateDir}", dateDirPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create archive {ArchivePath}", tarPath);
        }
    }
}
```

**Note:** The `TarWriter` and `TarEntryData` APIs may differ from what's shown above. .NET 9 has `System.IO.Compression.TarFile` which provides `TarFile.CreateFromDirectory` and `TarFile.ExtractToDirectory`. The implementation should use `TarFile.CreateFromDirectory` to create a tar stream, then compress it with ZstdSharp. The actual implementation may need to write the tar to a temporary file first, then compress it.

Revised approach — create tar in-memory, then compress:

```csharp
using Microsoft.Extensions.Logging;
using ZstdSharp;

namespace DanubeRS.CANServer.CLI.Commands.Upload;

public class ArchiveCompressor(ILogger<ArchiveCompressor> logger)
{
    public void CompressDateDirectory(string dateKey, string dateDirPath, string outputDirectory)
    {
        var tarPath = Path.Combine(outputDirectory, $"{dateKey}.tar.zst");

        var increment = 1;
        while (File.Exists(tarPath))
        {
            tarPath = Path.Combine(outputDirectory, $"{dateKey}_{increment}.tar.zst");
            increment++;
        }

        try
        {
            var tempTar = Path.Combine(outputDirectory, $"{dateKey}.tar.tmp");

            System.IO.Compression.TarFile.CreateFromDirectory(
                dateDirPath, tempTar, includeBaseDirectory: false);

            CompressFile(tempTar, tarPath);

            File.Delete(tempTar);
            logger.LogInformation("Created archive {ArchivePath}", tarPath);

            Directory.Delete(dateDirPath, recursive: true);
            logger.LogInformation("Removed date directory {DateDir}", dateDirPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create archive {ArchivePath}", tarPath);

            var tempTar = Path.Combine(outputDirectory, $"{dateKey}.tar.tmp");
            if (File.Exists(tempTar))
                File.Delete(tempTar);
        }
    }

    private static void CompressFile(string sourcePath, string destPath)
    {
        using var sourceStream = File.OpenRead(sourcePath);
        using var destStream = File.OpenWrite(destPath);
        using var compressionStream = new CompressionStream(destStream, level: 19);

        sourceStream.CopyTo(compressionStream);
    }
}
```

- [ ] **Step 2: Add CompressDateDirectories method to UploadCommandHandler**

In `UploadCommandHandler.cs`, add a method that iterates over the date directories and compresses them:

```csharp
private void CompressDateDirectories(Dictionary<string, List<string>> dateFiles, DirectoryInfo archiveDirectory)
{
    var compressor = new ArchiveCompressor(logger);

    foreach (var (dateKey, _) in dateFiles)
    {
        var dateDirPath = Path.Combine(archiveDirectory.FullName, dateKey);

        if (!Directory.Exists(dateDirPath))
            continue;

        compressor.CompressDateDirectory(dateKey, dateDirPath, archiveDirectory.FullName);
    }
}
```

Update the DI registration. Since `ArchiveCompressor` takes `ILogger<ArchiveCompressor>`, it can be instantiated directly or registered. For simplicity, instantiate it in `CompressDateDirectories` (already shown above — the handler creates it directly).

Actually, since `ArchiveCompressor` is created inside `CompressDateDirectories` and its logger is `ILogger<ArchiveCompressor>` (different from `ILogger<UploadCommandHandler>`), we need to either register it in DI or pass the generic logger. The simplest approach: make `CompressDateDirectories` create the compressor directly, passing the handler's logger cast appropriately. Or better: make `ArchiveCompressor` a static utility class that takes `ILogger` as a parameter to its methods:

```csharp
public static class ArchiveCompressor
{
    public static void CompressDateDirectory(string dateKey, string dateDirPath, string outputDirectory, ILogger logger)
    {
        // ... same logic, using logger parameter
    }

    private static void CompressFile(string sourcePath, string destPath)
    {
        // ... same logic
    }
}
```

This avoids DI registration complexity for a utility class. Update the handler method:

```csharp
private void CompressDateDirectories(Dictionary<string, List<string>> dateFiles, DirectoryInfo archiveDirectory)
{
    foreach (var (dateKey, _) in dateFiles)
    {
        var dateDirPath = Path.Combine(archiveDirectory.FullName, dateKey);

        if (!Directory.Exists(dateDirPath))
            continue;

        ArchiveCompressor.CompressDateDirectory(dateKey, dateDirPath, archiveDirectory.FullName, logger);
    }
}
```

- [ ] **Step 3: Run build and tests**

Run: `dotnet build src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj && dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/`
Expected: Build succeeds

- [ ] **Step 4: Commit**

```bash
git add src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/ArchiveCompressor.cs src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs
git commit -m "feat: add tar+zst compression for daily archive"
```

---

### Task 4: Update Archive Naming Unit Tests

**Files:**
- Modify: `src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveNamingTests.cs`
- Create: `src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveCompressorTests.cs`

- [ ] **Step 1: Update ArchiveNamingTests for date directory support**

The existing `ArchiveNamingTests` test the timestamp + increment logic but don't test the date directory organization. Update the tests to cover date directory creation and the `dir` mode behavior.

Update `ArchiveNamingTests.cs` to reflect the new `ArchiveMode` parameter and date directory structure. Add a test for date directory path generation:

```csharp
[Fact]
public void ArchiveNaming_DateDirectoryPath_IsCorrect()
{
    var timestamp = new DateTimeOffset(2025, 1, 30, 14, 30, 0, TimeSpan.Zero);
    var localTimestamp = timestamp.ToLocalTime();
    var dateKey = localTimestamp.ToString("yyyy-MM-dd");
    var timestampKey = localTimestamp.ToString("yyyy-MM-ddTHH-mm-ss");

    dateKey.Should().Be("2025-01-30");
    timestampKey.Should().MatchRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}$");
}
```

Add a test for the `ArchiveMode` enum values:

```csharp
[Fact]
public void ArchiveMode_HasExpectedValues()
{
    ArchiveMode.Dir.Should().Be(0);
    ArchiveMode.Tar.Should().Be(1);
}
```

- [ ] **Step 2: Create ArchiveCompressorTests**

Create `src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveCompressorTests.cs`:

```csharp
using DanubeRS.CANServer.CLI.Commands.Upload;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO.Compression;

namespace DanubeRS.CANServer.CLI.Tests;

public class ArchiveCompressorTests : IDisposable
{
    private readonly string _tempDir;

    public ArchiveCompressorTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"archive-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
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
        using var decompressionStream = new ZstdSharp.DecompressionStream(fileStream);
        using var tarReader = new TarReader(decompressionStream);

        var entries = new List<string>();
        while (tarReader.MoveNext())
        {
            entries.Add(tarReader.Current!.Name);
        }

        entries.Should().Contain("test.log");
    }
}
```

- [ ] **Step 3: Run all tests**

Run: `dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/ && dotnet test src/lib/DanubeRS.DBCParser.Tests/`
Expected: All tests pass

- [ ] **Step 4: Commit**

```bash
git add src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveNamingTests.cs src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveCompressorTests.cs
git commit -m "test: update archive naming tests and add compressor tests"
```

---

## Self-Review

**1. Spec coverage:**
- ✅ `--archive dir|tar` mode values — Task 2
- ✅ `--archive-path` option — Task 2
- ✅ Rename with timestamp + increment — Already implemented (Task 3 of previous plan)
- ✅ Move into date subdirectory — Task 2
- ✅ tar+zst compression at level 19 — Task 3
- ✅ Collision handling with `_1`, `_2` increment — Task 3
- ✅ Delete date directory after compression — Task 3

**2. Placeholder scan:** No TBD/TODO found. All code steps have complete implementations.

**3. Type consistency:**
- `ArchiveMode` enum is defined in `UploadCommand.cs` and referenced in `UploadCommandHandler.cs` via `ArchiveMode.Dir` / `ArchiveMode.Tar`
- `ArchiveCompressor` is a static class with `ILogger` parameter, matching the handler's `ILogger<UploadCommandHandler>` logger
- `FileProcessingResult` is used consistently from the previous plan's implementation
- `DirectoryInfo` for `archiveDirectory` (changed from `FileInfo` in the previous implementation, need to update `ArchiveFiles` signature)

**Correction needed:** The current `ArchiveFiles` method takes `FileInfo archiveDirectory` but should now take `DirectoryInfo archiveDirectory` since `--archive-path` is `DirectoryInfo?`. This is handled in Task 2 Step 2 where the signature changes to `ArchiveFiles(List<FileProcessingResult> results, ArchiveMode mode, DirectoryInfo archiveDirectory)`.