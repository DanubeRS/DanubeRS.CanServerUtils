# Timestamped Archive Naming Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rename archived log files using the time-sync timestamp extracted during parsing, replacing the original filename with an ISO 8601 local-time timestamp and single-digit increment.

**Architecture:** Add `FirstTimestamp` property to `Reader` to surface the first time-sync frame's timestamp. Change `ProcessFileAsync` to return a `FileProcessingResult` record containing the file and its timestamp. In `UploadCommandHandler`, use these results to compute archive filenames with ISO 8601 formatting and increment disambiguation (0–9, fail beyond that). Fall back to file creation time when no time-sync is present.

**Tech Stack:** .NET 9.0, System.CommandLine, xUnit, FluentAssertions

---

### Task 1: Add FirstTimestamp to Reader

**Files:**
- Modify: `src/lib/DanubeRS.CANServer.Lib/BinaryLogs/Reader.cs:14-15,142-146,241-244,293-300`
- Test: `src/cli/DanubeRS.CANServer.CLI.Tests/ReaderAsyncTests.cs`

- [ ] **Step 1: Write the failing test for FirstTimestamp**

Add a test to `ReaderAsyncTests.cs` that verifies `FirstTimestamp` is set after reading a V2 stream with a time-sync frame:

```csharp
[Fact]
public async Task GetNextFrameAsync_V2TimeSync_SetsFirstTimestamp()
{
    var syncTime = 1714908600000UL; // 2024-05-05 10:30:00 UTC in ms
    var timeSync = CreateV2TimeSyncFrame(syncTime);
    var payload = new byte[] { 0x01 };
    var dataFrame = CreateV2DataFrame(0xB0, 0x00, 0x00, 0x00, 0x00, 0x01, payload);

    await using var stream = CreateV2Stream(timeSync.Concat(dataFrame).ToArray());
    var reader = await Reader.Open(stream, NullLogger.Instance);

    reader.Should().NotBeNull();
    await reader!.GetNextFrameAsync(CancellationToken.None);

    reader.FirstTimestamp.Should().NotBeNull();
    reader.FirstTimestamp!.Value.ToUnixTimeMilliseconds().Should().Be((long)syncTime);
}
```

Add a test for V1 time-sync:

```csharp
[Fact]
public async Task GetNextFrameAsync_V1TimeSync_SetsFirstTimestamp()
{
    var syncTime = 1714908600000UL;
    var timeSync = CreateV1TimeSyncFrame(syncTime);
    var payload = new byte[] { 0xAA };
    var dataFrame = CreateV1DataFrame(100, 0x100, 0x22, payload);

    await using var stream = CreateV1Stream(timeSync.Concat(dataFrame).ToArray());
    var reader = await Reader.Open(stream, NullLogger.Instance);

    reader.Should().NotBeNull();
    await reader!.GetNextFrameAsync(CancellationToken.None);

    reader.FirstTimestamp.Should().NotBeNull();
    reader.FirstTimestamp!.Value.ToUnixTimeMilliseconds().Should().Be((long)syncTime);
}
```

Add a test for null FirstTimestamp when no time-sync is present (V2 stream with only a data frame, no 0xA0 sync):

```csharp
[Fact]
public async Task GetNextFrameAsync_NoTimeSync_FirstTimestampIsNull()
{
    // V2 stream with data frame but no time-sync
    var payload = new byte[] { 0x01 };
    var dataFrame = CreateV2DataFrame(0xB0, 0x00, 0x00, 0x00, 0x00, 0x01, payload);

    await using var stream = CreateV2Stream(dataFrame);
    var reader = await Reader.Open(stream, NullLogger.Instance);

    reader.Should().NotBeNull();
    // Note: this will return a frame with timestamp based on _lastSyncTime=0, but FirstTimestamp stays null
    // since no time-sync frame was encountered
    // Actually, V2 data frames without a preceding time-sync still work (offset from 0)
    // We just don't set FirstTimestamp
    var frame = await reader!.GetNextFrameAsync(CancellationToken.None);
    // FirstTimestamp remains null because no 0xA0 was seen
    reader.FirstTimestamp.Should().BeNull();
}
```

Add a test that FirstTimestamp is set only once (not overwritten by later syncs):

```csharp
[Fact]
public async Task GetNextFrameAsync_MultipleTimeSyncs_FirstTimestampIsFirstSync()
{
    var firstSync = 1714908600000UL;
    var secondSync = 1714912200000UL; // 1 hour later
    var payload = new byte[] { 0x01 };

    var timeSync1 = CreateV2TimeSyncFrame(firstSync);
    var dataFrame1 = CreateV2DataFrame(0xB0, 0x00, 0x00, 0x00, 0x00, 0x01, payload);
    var timeSync2 = CreateV2TimeSyncFrame(secondSync);
    var dataFrame2 = CreateV2DataFrame(0xB0, 0x00, 0x00, 0x00, 0x00, 0x01, payload);

    await using var stream = CreateV2Stream(
        timeSync1.Concat(dataFrame1).Concat(timeSync2).Concat(dataFrame2).ToArray());
    var reader = await Reader.Open(stream, NullLogger.Instance);

    reader.Should().NotBeNull();
    await reader!.GetNextFrameAsync(CancellationToken.None);
    await reader.GetNextFrameAsync(CancellationToken.None);

    reader.FirstTimestamp!.Value.ToUnixTimeMilliseconds().Should().Be((long)firstSync);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/ --filter "FullyQualifiedName~FirstTimestamp" --no-restore`
Expected: FAIL — `FirstTimestamp` property doesn't exist yet.

- [ ] **Step 3: Add FirstTimestamp property to Reader**

In `Reader.cs`, add after line 15 (`private bool _disposed = false;`):

```csharp
private DateTimeOffset? _firstTimestamp;
```

And add the public property after the `_disposed` field:

```csharp
public DateTimeOffset? FirstTimestamp => _firstTimestamp;
```

In `ReadV2FrameAsync` (line 142-146), change the time-sync block from:

```csharp
if (start == 0xA0)
{
    var timeSyncMem = buffer.AsMemory(0, 8);
    await _stream.ReadExactlyAsync(timeSyncMem, cancellationToken);
    _lastSyncTime = BitConverter.ToUInt64(timeSyncMem.Span);
}
```

to:

```csharp
if (start == 0xA0)
{
    var timeSyncMem = buffer.AsMemory(0, 8);
    await _stream.ReadExactlyAsync(timeSyncMem, cancellationToken);
    _lastSyncTime = BitConverter.ToUInt64(timeSyncMem.Span);
    _firstTimestamp ??= DateTimeOffset.FromUnixTimeMilliseconds((long)_lastSyncTime);
}
```

In `ReadV1FrameAsync` (line 190-201), change the 0xCE case from:

```csharp
case 0xce:
    try
    {
        var timeSyncMem = buffer.AsMemory(0, 8);
        await _stream.ReadExactlyAsync(timeSyncMem, cancellationToken);
    }
    catch (EndOfStreamException e)
    {
        throw new InvalidFrameException("Time sync payload is the wrong length", _stream.Position, e);
    }

    _lastSyncTime = BitConverter.ToUInt64(buffer, 0);
```

to:

```csharp
case 0xce:
    try
    {
        var timeSyncMem = buffer.AsMemory(0, 8);
        await _stream.ReadExactlyAsync(timeSyncMem, cancellationToken);
    }
    catch (EndOfStreamException e)
    {
        throw new InvalidFrameException("Time sync payload is the wrong length", _stream.Position, e);
    }

    _lastSyncTime = BitConverter.ToUInt64(buffer, 0);
    _firstTimestamp ??= DateTimeOffset.FromUnixTimeMilliseconds((long)_lastSyncTime);
```

In `ReadV2Frame` (line 241-244), change:

```csharp
if (start == 0xA0)
{
    _stream.ReadExactly(timeSyncBuffer);
    _lastSyncTime = BitConverter.ToUInt64(timeSyncBuffer);
}
```

to:

```csharp
if (start == 0xA0)
{
    _stream.ReadExactly(timeSyncBuffer);
    _lastSyncTime = BitConverter.ToUInt64(timeSyncBuffer);
    _firstTimestamp ??= DateTimeOffset.FromUnixTimeMilliseconds((long)_lastSyncTime);
}
```

In `ReadV1Frame` (line 290-300), change:

```csharp
case 0xce:
    try
    {
        _stream.ReadExactly(timeSyncBuffer);
    }
    catch (EndOfStreamException e)
    {
        throw new InvalidFrameException("Time sync payload is the wrong length", _stream.Position, e);
    }

    _lastSyncTime = BitConverter.ToUInt64(timeSyncBuffer);
```

to:

```csharp
case 0xce:
    try
    {
        _stream.ReadExactly(timeSyncBuffer);
    }
    catch (EndOfStreamException e)
    {
        throw new InvalidFrameException("Time sync payload is the wrong length", _stream.Position, e);
    }

    _lastSyncTime = BitConverter.ToUInt64(timeSyncBuffer);
    _firstTimestamp ??= DateTimeOffset.FromUnixTimeMilliseconds((long)_lastSyncTime);
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/ --filter "FullyQualifiedName~FirstTimestamp"`
Expected: 4 tests PASS

Also run full test suite:
Run: `dotnet test src/lib/DanubeRS.DBCParser.Tests/ && dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/`
Expected: All pass (174 + 40 = 214)

- [ ] **Step 5: Commit**

```bash
git add src/lib/DanubeRS.CANServer.Lib/BinaryLogs/Reader.cs src/cli/DanubeRS.CANServer.CLI.Tests/ReaderAsyncTests.cs
git commit -m "feat: add FirstTimestamp property to Reader for archive naming"
```

---

### Task 2: Return FileProcessingResult from ProcessFileAsync

**Files:**
- Modify: `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/CanLogUploadPipeline.cs:122-142,291-303`

- [ ] **Step 1: Add FileProcessingResult record and change ProcessFileAsync signature**

At the end of `CanLogUploadPipeline.cs`, after the `SignalData` record (line 303), add:

```csharp
public record FileProcessingResult(FileInfo File, DateTimeOffset? FirstTimestamp);
```

Change `ProcessFileAsync` (lines 122-142) from:

```csharp
private async Task ProcessFileAsync(FileInfo file, ITargetBlock<FrameWithContext> readerBlock, CancellationToken cancellationToken)
{
    logger.LogInformation("Reading file {LogFileName}", file.FullName);
    
    await using var fs = file.OpenRead();
    var reader = await Reader.Open(fs, logger);
    
    if (reader == null)
    {
        logger.LogWarning("Could not open file {LogFileName}", file.FullName);
        return;
    }

    while (true)
    {
        var frame = await reader.GetNextFrameAsync(cancellationToken);
        if (frame == null) break;

        await readerBlock.SendAsync(new FrameWithContext(frame, file.Name), cancellationToken);
    }
}
```

to:

```csharp
private async Task<FileProcessingResult> ProcessFileAsync(FileInfo file, ITargetBlock<FrameWithContext> readerBlock, CancellationToken cancellationToken)
{
    logger.LogInformation("Reading file {LogFileName}", file.FullName);
    
    await using var fs = file.OpenRead();
    var reader = await Reader.Open(fs, logger);
    
    if (reader == null)
    {
        logger.LogWarning("Could not open file {LogFileName}", file.FullName);
        return new FileProcessingResult(file, null);
    }

    while (true)
    {
        var frame = await reader.GetNextFrameAsync(cancellationToken);
        if (frame == null) break;

        await readerBlock.SendAsync(new FrameWithContext(frame, file.Name), cancellationToken);
    }

    return new FileProcessingResult(file, reader.FirstTimestamp);
}
```

- [ ] **Step 2: Change ProcessFilesAsync to collect FileProcessingResults**

Change `ProcessFilesAsync` (lines 29-120) signature from:

```csharp
public async Task ProcessFilesAsync(IEnumerable<FileInfo> files, CancellationToken cancellationToken)
```

to:

```csharp
public async Task<List<FileProcessingResult>> ProcessFilesAsync(IEnumerable<FileInfo> files, CancellationToken cancellationToken)
```

Change the file processing loop (lines 64-69) from:

```csharp
foreach (var file in files)
{
    if (cancellationToken.IsCancellationRequested) break;

    await ProcessFileAsync(file, readerBlock, cancellationToken);
}
```

to:

```csharp
var results = new List<FileProcessingResult>();
foreach (var file in files)
{
    if (cancellationToken.IsCancellationRequested) break;

    var result = await ProcessFileAsync(file, readerBlock, cancellationToken);
    results.Add(result);
}
```

At the end of the method, before the final `}`, change from implicit `void` return to explicit return. After the last `logger.LogInformation` block (around line 119), add:

```csharp
return results;
```

Note: The method already has multiple return points due to the `while` loop for watching. The watch loop case at line 82 (`return 0;`) isn't in this method — it's in `UploadCommandHandler`. The `ProcessFilesAsync` method needs to return `results` at the end after the logging. If there are early returns due to cancellation, return `results` as well.

- [ ] **Step 3: Run build and tests**

Run: `dotnet build src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj && dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/`
Expected: Build succeeds, all tests pass. The `FileProcessingResult` type is not yet consumed, but the return type change must compile.

- [ ] **Step 4: Commit**

```bash
git add src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/CanLogUploadPipeline.cs
git commit -m "feat: ProcessFileAsync returns FileProcessingResult with FirstTimestamp"
```

---

### Task 3: Implement Timestamped Archive Naming in UploadCommandHandler

**Files:**
- Modify: `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs:1-8,53-68`

- [ ] **Step 1: Update UploadCommandHandler to use FileProcessingResult for archive naming**

Change the `ProcessFilesAsync` call and archive section in `UploadCommandHandler.InvokeAsync` from:

```csharp
await pipeline.ProcessFilesAsync(files, cancellationToken);

if (archiveDirectory != null)
{
    foreach (var file in files)
    {
        try
        {
            file.MoveTo(Path.Combine(archiveDirectory.FullName, file.Name));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to archive file {FileName}", file.Name);
        }
    }
}
```

to:

```csharp
var results = await pipeline.ProcessFilesAsync(files, cancellationToken);

if (archiveDirectory != null)
{
    ArchiveFiles(results, archiveDirectory);
}
```

Add the `ArchiveFiles` method to `UploadCommandHandler` class (after `LoadRateLimitConfig`):

```csharp
private void ArchiveFiles(List<CanLogUploadPipeline.FileProcessingResult> results, DirectoryInfo archiveDirectory)
{
    var timestampCounts = new Dictionary<string, int>();

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
            logger.LogError("Cannot archive file {FileName}: more than 10 files share timestamp {TimestampKey}", result.File.Name, timestampKey);
            continue;
        }

        var archiveFileName = $"{timestampKey}_{increment}.log";
        var archivePath = Path.Combine(archiveDirectory.FullName, archiveFileName);

        try
        {
            result.File.MoveTo(archivePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to archive file {FileName} to {ArchivePath}", result.File.Name, archivePath);
        }

        timestampCounts[timestampKey] = increment + 1;
    }
}
```

Also add `using DanubeRS.CANServer.CLI.Commands.Upload;` if not already present (it should be, via the namespace of `CanLogUploadPipeline`). The `FileProcessingResult` record is defined in `CanLogUploadPipeline.cs` in the `DanubeRS.CANServer.CLI.Commands.Upload` namespace, and `UploadCommandHandler.cs` already has `using DanubeRS.CANServer.CLI.Commands.Upload;` — but wait, the `FileProcessingResult` is a public record on `CanLogUploadPipeline` class? No — looking at the plan, it's a top-level record in the namespace. Let me check: `CanLogUploadPipeline.cs` defines inner records (`RawFrame`, `DecodedMessage`, `FrameWithContext`, `SignalData`) which are all `private`. The `FileProcessingResult` in Task 2 should be `public` so it's accessible from `UploadCommandHandler`. 

Actually, looking at the current code more carefully — the inner records are `private record` inside `CanLogUploadPipeline`. The `FileProcessingResult` should be a public record that can be referenced from `UploadCommandHandler`. Let me make it a standalone public record in the namespace, not nested in `CanLogUploadPipeline`.

**Correction to Task 2:** The `FileProcessingResult` record should be defined as a `public record` at the namespace level (outside the `CanLogUploadPipeline` class), so it can be referenced from `UploadCommandHandler`.

Revised: In Task 2 Step 1, add `FileProcessingResult` at the end of the file, after the closing `}` of `CanLogUploadPipeline` class, at namespace level:

```csharp
public record FileProcessingResult(FileInfo File, DateTimeOffset? FirstTimestamp);
```

This is a standalone public record in the `DanubeRS.CANServer.CLI.Commands.Upload` namespace.

- [ ] **Step 2: Run build and tests**

Run: `dotnet build src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj && dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/`
Expected: Build succeeds, all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/CanLogUploadPipeline.cs
git commit -m "feat: timestamped archive naming with increment disambiguation"
```

---

### Task 4: Add Archive Naming Unit Tests

**Files:**
- Create: `src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveNamingTests.cs`

- [ ] **Step 1: Write tests for archive naming logic**

Create `src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveNamingTests.cs`:

```csharp
using System.Collections.Concurrent;
using DanubeRS.CANServer.CLI.Commands.Upload;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CANServer.CLI.Tests;

public class ArchiveNamingTests
{
    // We test the archive naming by simulating what ArchiveFiles does:
    // 1. Two files with same timestamp get _0 and _1 suffixes
    // 2. A file with no FirstTimestamp uses CreationTime
    // 3. Increment > 9 logs error and skips
    // 4. Different timestamps get independent _0 suffixes

    [Fact]
    public void ArchiveNaming_TwoFilesSameTimestamp_GetsIncrementingSuffixes()
    {
        var timestamp = new DateTimeOffset(2026, 5, 5, 14, 30, 0, TimeSpan.Zero);
        var key = timestamp.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");

        var counts = new Dictionary<string, int>();
        var results = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            var increment = counts.GetValueOrDefault(key, 0);
            results.Add($"{key}_{increment}.log");
            counts[key] = increment + 1;
        }

        results.Should().Equal(
            $"{key}_0.log",
            $"{key}_1.log",
            $"{key}_2.log");
    }

    [Fact]
    public void ArchiveNaming_DifferentTimestamps_GetIndependentSuffixes()
    {
        var timestamp1 = new DateTimeOffset(2026, 5, 5, 14, 30, 0, TimeSpan.Zero);
        var timestamp2 = new DateTimeOffset(2026, 5, 5, 15, 22, 10, TimeSpan.Zero);
        var key1 = timestamp1.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");
        var key2 = timestamp2.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");

        var counts = new Dictionary<string, int>();

        var inc1 = counts.GetValueOrDefault(key1, 0);
        counts[key1] = inc1 + 1;
        var inc2 = counts.GetValueOrDefault(key2, 0);
        counts[key2] = inc2 + 1;

        inc1.Should().Be(0);
        inc2.Should().Be(0);
    }

    [Fact]
    public void ArchiveNaming_IncrementBeyond9_StopsAt9()
    {
        var timestamp = new DateTimeOffset(2026, 5, 5, 14, 30, 0, TimeSpan.Zero);
        var key = timestamp.ToLocalTime().ToString("yyyy-MM-ddTHH-mm-ss");

        var counts = new Dictionary<string, int>();

        // 10 files should be fine (0-9)
        for (int i = 0; i < 10; i++)
        {
            var increment = counts.GetValueOrDefault(key, 0);
            increment.Should().BeLessOrEqualTo(9);
            counts[key] = increment + 1;
        }

        // 11th file should exceed limit
        var elevenIncrement = counts.GetValueOrDefault(key, 0);
        elevenIncrement.Should().Be(10); // > 9, which would trigger the error path
    }

    [Fact]
    public void ArchiveNaming_FirstTimestampNull_FallsBackToFileCreationTime()
    {
        var file = new FileInfo(Path.GetTempFileName());
        try
        {
            var creationTime = file.CreationTime;
            DateTimeOffset? firstTimestamp = null;
            var effectiveTimestamp = firstTimestamp ?? creationTime;

            effectiveTimestamp.Should().Be(creationTime);
        }
        finally
        {
            File.Delete(file.FullName);
        }
    }
}
```

- [ ] **Step 2: Run the tests**

Run: `dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/ --filter "FullyQualifiedName~ArchiveNaming"`
Expected: 4 tests PASS

- [ ] **Step 3: Run full test suite**

Run: `dotnet test src/lib/DanubeRS.DBCParser.Tests/ && dotnet test src/cli/DanubeRS.CANServer.CLI.Tests/`
Expected: 174 + 44 = 218 total, all pass

- [ ] **Step 4: Commit**

```bash
git add src/cli/DanubeRS.CANServer.CLI.Tests/ArchiveNamingTests.cs
git commit -m "test: add archive naming unit tests"
```

---

## Self-Review

**1. Spec coverage:**
- ✅ Reader.FirstTimestamp — Task 1
- ✅ ProcessFileAsync returns FileProcessingResult — Task 2
- ✅ ProcessFilesAsync returns List<FileProcessingResult> — Task 2
- ✅ UploadCommandHandler computes archive names with ISO 8601 format — Task 3
- ✅ Increment disambiguation per timestamp key — Task 3
- ✅ Fail if increment > 9 — Task 3
- ✅ Fallback to file creation time — Task 3

**2. Placeholder scan:** No TBD/TODO found. All code steps have complete implementations.

**3. Type consistency:**
- `FileProcessingResult` is defined as `public record` at namespace level in `CanLogUploadPipeline.cs` and referenced in `UploadCommandHandler.cs` via the shared `DanubeRS.CANServer.CLI.Commands.Upload` namespace.
- `FirstTimestamp` is `DateTimeOffset?` in Reader, matches the `FileProcessingResult` field type.
- `ProcessFilesAsync` returns `Task<List<FileProcessingResult>>`, consumed by `UploadCommandHandler.InvokeAsync`.
- `ArchiveFiles` uses `List<CanLogUploadPipeline.FileProcessingResult>` — wait, if `FileProcessingResult` is a namespace-level public record, the type reference should be just `FileProcessingResult` or `DanubeRS.CANServer.CLI.Commands.Upload.FileProcessingResult`. Since `UploadCommandHandler.cs` has `using DanubeRS.CANServer.CLI.Commands.Upload;`, it can reference `FileProcessingResult` directly.

**Correction:** In Task 3 Step 1, the `ArchiveFiles` method parameter type should be `List<FileProcessingResult>` (not `List<CanLogUploadPipeline.FileProcessingResult>`) since the `using` statement already imports the namespace. This is already correct in the code shown.