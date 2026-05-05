# Enhanced Archive for Upload Command

## Goal

After uploading CAN log files to InfluxDB, rename processed files with timestamps extracted from the binary log, organize them into date directories, and optionally compress each day's files into a zstandard archive.

## Architecture

The `upload` command's `--archive` option gains two modes: `dir` (move to date directories) and `tar` (move to date directories, then compress into daily `.tar.zst` archives). The flow runs after `ProcessFilesAsync` completes: compute timestamp, rename, move to date directory, then optionally tar+compress.

## Design

### 1. `--archive` option

Changes from a boolean/flag to accept a mode value:

- `--archive dir` — rename files with timestamps, move into date subdirectories
- `--archive tar` — rename files with timestamps, move into date subdirectories, then compress each date directory into a `.tar.zst` archive and delete the date directory

When `--archive` is not specified, no archiving happens (files stay in place after upload).

### 2. `--archive-path` option

Destination directory for both modes. Defaults to the input directory if not specified.

### 3. Post-upload archive flow

After `ProcessFilesAsync` returns `List<FileProcessingResult>`:

1. **Compute timestamped name** for each file: `FirstTimestamp ?? File.CreationTime` → `yyyy-MM-ddTHH-mm-ss_N.log`
2. **Rename** the file in-place to the timestamped name
3. **Move** the renamed file into a date subdirectory: `{archive-path}/yyyy-MM-dd/yyyy-MM-ddTHH-mm-ss_N.log`
4. **If `tar` mode**: compress each date directory into `{archive-path}/yyyy-MM-dd.tar.zst`, then delete the date directory

### 4. Filename collision handling

- **Timestamp increment**: per timestamp group, `_0` through `_9`. Error if > 9 files share the same timestamp.
- **`dir` mode date directory collision**: files just move into the existing directory — filenames are already unique due to timestamp+increment.
- **`tar` mode archive collision**: if `yyyy-MM-dd.tar.zst` already exists, create `yyyy-MM-dd_1.tar.zst`, `yyyy-MM-dd_2.tar.zst`, etc.

### 5. Compression

Zstandard at compression level 19 (high). Hardcoded, no user-configurable option.

### 6. Directory structure examples

`dir` mode:
```
archive/
  2025-01-30/
    2025-01-30T14-30-00_0.log
    2025-01-30T14-30-00_1.log
  2025-01-31/
    2025-01-31T08-15-22_0.log
```

`tar` mode:
```
archive/
  2025-01-30.tar.zst
  2025-01-31.tar.zst
```

### 7. Error handling

- Each file's rename and move is wrapped in individual try/catch
- If a date directory cannot be created, log error and skip that file
- If tar.zst creation fails for a date directory, log error and leave the date directory in place (don't delete files)
- If increment > 9, log error and skip that file

## Files

- `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommand.cs` — Change `--archive` from `FileInfo?` to mode enum, add `--archive-path` option
- `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/UploadCommandHandler.cs` — Replace `ArchiveFiles` with new archive flow (rename → date dir → optional tar.zst)
- `src/cli/DanubeRS.CANServer.CLI/Cli/Commands/Upload/CanLogUploadPipeline.cs` — `FileProcessingResult` already provides `FirstTimestamp` (no change needed)