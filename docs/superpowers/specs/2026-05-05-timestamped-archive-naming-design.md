# Timestamped Archive Naming

## Goal

Rename archived log files using the time-sync timestamp extracted during parsing, replacing the original filename with an ISO 8601 local-time timestamp and a single-digit increment.

## Architecture

Surfaces the first time-sync timestamp from `Reader` via a new public property. `ProcessFileAsync` returns per-file metadata (file + timestamp) back to `UploadCommandHandler`, which computes archive filenames using ISO 8601 format with increment disambiguation. Falls back to file creation time when no time-sync is present.

## Design

### 1. Reader.FirstTimestamp

Add `public DateTimeOffset? FirstTimestamp { get; private set; }` to `Reader`.

Set once when `_lastSyncTime` is first assigned — in both `ReadV2Frame`/`ReadV2FrameAsync` (0xA0 time-sync) and `ReadV1Frame`/`ReadV1FrameAsync` (0xCE time-sync). Subsequent time-syncs update `_lastSyncTime` but not `FirstTimestamp`.

The timestamp is computed as `DateTimeOffset.FromUnixTimeMilliseconds((long)_lastSyncTime)` and converted to local time.

If no time-sync frame appears in the file, `FirstTimestamp` remains null.

### 2. ProcessFileAsync returns FileProcessingResult

Change `ProcessFileAsync` return type from `Task` to `Task<FileProcessingResult>`:

```csharp
private record FileProcessingResult(FileInfo File, DateTimeOffset? FirstTimestamp);
```

After the reader loop completes, read `reader.FirstTimestamp` and return it. `ProcessFilesAsync` collects these results into a list and returns them to the caller.

`ProcessFilesAsync` signature changes from `Task` to `Task<List<FileProcessingResult>>`.

### 3. UploadCommandHandler archive naming

After `ProcessFilesAsync` completes, if `archiveDirectory` is set, compute archive names for each result:

1. Timestamp: `result.FirstTimestamp ?? result.File.CreationTime`
2. Format: `yyyy-MM-ddTHH-mm-ss` (ISO 8601 local time, second precision)
3. Track increments per timestamp key using `Dictionary<string, int>`
4. Final filename: `{timestamp}_{increment}.log`
5. If increment > 9, log error and skip that file
6. Each `MoveTo` wrapped in individual try/catch (existing behavior preserved)

Example filenames:
- `2026-05-05T14-30-00_0.log`
- `2026-05-05T14-30-00_1.log`
- `2026-05-05T15-22-10_0.log`

### 4. Fallback behavior

When `FirstTimestamp` is null (file had no time-sync frame), use `FileInfo.CreationTime` as the timestamp. This ensures every file gets a valid archive name.

## Files

- `src/lib/DanubeRS.CANServer.Lib/BinaryLogs/Reader.cs` — Add `FirstTimestamp` property, set in sync/async time-sync paths
- `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/CanLogUploadPipeline.cs` — `ProcessFileAsync` returns `FileProcessingResult`, `ProcessFilesAsync` returns `List<FileProcessingResult>`
- `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs` — Compute archive names from timestamps with increment logic