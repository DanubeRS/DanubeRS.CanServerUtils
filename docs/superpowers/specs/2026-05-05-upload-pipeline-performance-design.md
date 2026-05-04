# Upload Pipeline Performance: Approach A — Pipeline Reuse + Allocation Reduction

## Goal

Improve CAN log upload throughput (points/sec) by eliminating per-file pipeline recreation, reducing hot-path allocations, fixing a data-loss bug, and adding async I/O.

## Context

The upload command processes many small `.log` files through a TPL Dataflow pipeline (Reader → Decoder → FilterConverter → RateLimiter → Batcher → Uploader). Current bottlenecks:

- Pipeline recreated per file (loses cross-file batching, wastes block setup)
- `BitArray` allocated per `TryParseBinaryMessage` call (GC pressure on hot path)
- Synchronous `Reader.GetNextFrame()` blocks thread pool threads
- `SignalRateLimiter.Flush()` silently discards remaining window data
- `ConcurrentBag<DecodeError>` grows unbounded (only count is used)
- `RateLimitResolver` does linear search per signal per frame

## Design Sections

### 1. Pipeline Reuse Across Files

**Current**: `UploadCommandHandler.ParseFileInternal()` creates a new `CanLogUploadPipeline` per file. Each pipeline gets fresh TPL blocks; the batcher/uploader lose cross-file batching.

**Change**: Create one pipeline outside the per-file loop. Call `ProcessFilesAsync(allFiles)` once. Also, `LoadRateLimitConfig` is called per file but config doesn't change — load once.

**Before**:
```
foreach file → ParseFileInternal → new CanLogUploadPipeline → ProcessFilesAsync([single file])
```

**After**:
```
LoadRateLimitConfig once
var pipeline = new CanLogUploadPipeline(...)
pipeline.ProcessFilesAsync(allFiles, cancellationToken)
```

**Watch loop interaction**: Currently the watch loop re-creates a pipeline per file on each pass. With a shared pipeline, each watch loop iteration should enumerate new files and feed them through the same pipeline instance. The pipeline `ProcessFilesAsync` already loops over files internally, so the handler's watch loop becomes:
1. Enumerate `*.log` files
2. If files exist, create pipeline (once, on first iteration) and call `ProcessFilesAsync(files)`
3. If `--watch` is set, wait, then enumerate again and call `ProcessFilesAsync(newFiles)` on a **new** pipeline instance (since the previous pipeline's TPL blocks are completed)

This means the pipeline is reused **within a single watch iteration** (all files in that batch), but a new pipeline is created per watch iteration. This is acceptable since watch intervals are typically 60s — pipeline creation cost is negligible at that cadence.

**Archive handling**: After `ProcessFilesAsync` completes for a batch, the handler archives the processed files. Error handling per-file stays in the handler's try/catch around `ProcessFilesAsync` — if a file throws, the whole batch fails. This is unchanged behavior.

**Files**: `UploadCommandHandler.cs` — remove `ParseFileInternal`, restructure `InvokeAsync`

### 2. Eliminate BitArray from Decode Path

**Current**: `Database.TryParseBinaryMessage()` (`Database.cs:107`) creates `new BitArray(data)` every call. `BitArray` internally allocates an `int[]` and copies bits — expensive on the hot path.

**Change**: Replace `BitArray` with span-based bit access. Add a `GetBit` helper and refactor `Decoder` methods to accept `ReadOnlySpan<byte>` instead of `BitArray`.

```csharp
static bool GetBit(ReadOnlySpan<byte> data, int bitIndex)
{
    return (data[bitIndex >> 3] & (1 << (bitIndex & 7))) != 0;
}
```

- `ExtractBitsLittleEndian(BitArray, ...)` → `ExtractBitsLittleEndian(ReadOnlySpan<byte>, ...)`
- `ExtractBitsBigEndian(BitArray, ...)` → `ExtractBitsBigEndian(ReadOnlySpan<byte>, ...)`
- `TryParseRegularMessage` and `TryParseMultiplexedMessage` take `ReadOnlySpan<byte> data` instead of `BitArray`
- `Database.TryParseBinaryMessage` passes `data` directly without creating `BitArray`. Signature becomes: `TryParseBinaryMessage(uint frameId, byte[] data, [NotNullWhen(true)] out MessageValue? value, [NotNullWhen(true)] out MessageDefinition? defn)` — unchanged externally, but internally no `BitArray` is created. The `data.Length * 8` replaces `bitArray.Length` for bounds checking.
- `GetBitArrayDataForLog` (trace logging only) can still create `BitArray` when needed, gated by `IsEnabled(LogLevel.Trace)`
- `Decoder.GetSignalValue` changes from `BitArray data` to `ReadOnlySpan<byte> data`. Internal bounds checks change from `startBit + i < bitArray.Length` to `startBit + i < data.Length * 8`
- `TryParseRegularMessage` / `TryParseMultiplexedMessage` bounds checks change from `lastSignal.StartBit + lastSignal.Size > data.Length` to `lastSignal.StartBit + lastSignal.Size > data.Length * 8` (bit count from byte count)

**Files**: `Database.cs`, `Decoder.cs`

### 3. Async Reader — GetNextFrameAsync

**Current**: `Reader.GetNextFrame()` uses synchronous `Stream.Read`/`ReadExactly`, blocking a thread pool thread inside async `ProcessFileAsync`.

**Change**: Add `GetNextFrameAsync(CancellationToken)` method using `ReadAsync`/`ReadExactlyAsync`. The `stackalloc` buffers (5-8 bytes) in `ReadV2Frame`/`ReadV1Frame` must become `ArrayPool`-rented for async compatibility.

```csharp
public async Task<LogReaderFrame?> GetNextFrameAsync(CancellationToken cancellationToken)
```

**ReadByte → async mapping**: `Stream` has no `ReadByteAsync`. Replace with a single-byte `Memory<byte>` read:
```csharp
var singleByte = new byte[1];
var read = await _stream.ReadAsync(singleByte, cancellationToken);
if (read == 0) return -1; // EOF
return singleByte[0];
```
For the outer loop that repeatedly calls `ReadByte` to find frame start bytes, this means each iteration allocates a 1-byte buffer. Mitigate by renting a small buffer from `ArrayPool<byte>.Shared` once per `GetNextFrameAsync` call and slicing it:
```csharp
var buffer = ArrayPool<byte>.Shared.Rent(16); // reuse for all small reads within one call
try { ... } finally { ArrayPool<byte>.Shared.Return(buffer); }
```

**ArrayPool rental pattern**: The `stackalloc` buffers (`timeSyncBuffer[8]`, `frameDataBuffer[5]`) in `ReadV2FrameAsync`/`ReadV1FrameAsync` become `ArrayPool<byte>.Shared.Rent(8)` and `ArrayPool<byte>.Shared.Rent(5)` respectively. Since `ArrayPool` rounds up to power-of-2 bucket sizes, both will likely rent 16 bytes. Return in a `finally` block. The actual frame payload stays as `new byte[frameLength]` (returned in `LogReaderFrame`).

Keep `GetNextFrame()` for backward compatibility. `CanLogUploadPipeline.ProcessFileAsync` switches to calling the async version.

**Files**: `Reader.cs`, `CanLogUploadPipeline.cs`

### 4. Replace ConcurrentBag<DecodeError> with Bounded Counter

**Current**: `_decodeErrors` (`CanLogUploadPipeline.cs:24`) is a `ConcurrentBag<DecodeError>` storing every failed frame. Only `_decodeErrors.Count` is used at the end.

**Change**: Replace with `long _decodeErrorCount` + `Interlocked.Increment`. Optionally track first 10 unique failed frame IDs via a small `ConcurrentDictionary<uint, byte>` for diagnostic value.

Remove `DecodeError` record. Remove `Errors` property.

**Files**: `CanLogUploadPipeline.cs`

### 5. Fix SignalRateLimiter.Flush() — Data Loss Bug

**Current**: `Flush()` aggregates remaining window values and increments counters, but never returns `PointData`. Aggregated values are computed and discarded.

**Change**:
1. Extend `SignalWindowState` to store last-known metadata (`SignalName`, `Unit`, `Tags`, `LastTimestamp`) so `Flush()` can reconstruct `PointData`.
2. Change `Flush()` return type to `List<PointData>`.
3. In `CanLogUploadPipeline.ProcessFilesAsync`, post flushed points to the batcher block after `readerBlock.Complete()`.

**Metadata storage in `TryProcessPoint`**: When `TryProcessPoint` creates a new `SignalWindowState` via `GetOrAdd`, or when a window closes and a new state is created (`_state[key] = new SignalWindowState(...)`), the metadata fields (`SignalName`, `Unit`, `Tags`, `LastTimestamp`) must be populated from the current call's parameters. This ensures `Flush()` always has enough context to build a `PointData` for any remaining window. The `Tags` dictionary should be stored by reference (it's already a reference type) — no deep copy needed since the pipeline's filter/converter stage creates a fresh `Dictionary` per signal.

**Files**: `SignalRateLimiter.cs`, `CanLogUploadPipeline.cs`

### 6. RateLimitResolver Lookup Optimization

**Current**: `RateLimitResolver.GetRule()` does linear search through `Frames` list per call.

**Change**: Pre-build `Dictionary<uint, FrameRule>` keyed by frame ID at construction. O(1) frame lookup, signal-level stays linear (typically <20 signals per frame).

**Files**: `RateLimitResolver.cs`

## Implementation Order & Dependencies

```
1. Pipeline Reuse (no deps)
2. BitArray Elimination (no deps)
3. Async Reader (no deps, but 1 should be done first to avoid conflicts in CanLogUploadPipeline.cs)
4. DecodeError Counter (no deps)
5. Flush() Bug Fix (no deps, but touches same files as 1 and 3)
6. Resolver Lookup (no deps)

Recommended order: 1 → 2 → 3 → 6 → 4 → 5
(Do pipeline reuse first since it restructures UploadCommandHandler; BitArray next since it's the biggest win and touches different files; async reader third since it touches Reader.cs independently; resolver and counter are simple; flush fix last since it touches CanLogUploadPipeline.cs which 1 and 3 also touch)
```

## Testing Strategy

### BitArray Elimination (highest risk)
- Existing `DecodeTests` in `DanubeRS.DBCParser.Tests` decode known CAN frames — these must continue to pass unchanged since the public API contract (`TryParseBinaryMessage` returns same results) doesn't change
- Add a direct comparison test: decode the same frame data with old `BitArray` path and new span-based path, assert identical `SignalValue` results across multiple frame IDs
- Test edge cases: 0-byte payloads, 8-byte payloads, signals at bit boundaries (start bit 0, start bit 63), multiplexed frames

### Pipeline Reuse
- Verify watch loop still processes new files correctly (integration-style test or manual)
- Verify error handling per-file doesn't kill the shared pipeline

### Async Reader
- Compare `GetNextFrameAsync` output against `GetNextFrame` for the same file
- Test cancellation token propagation

### Flush() Bug Fix
- Create a rate-limited scenario with partial windows at end, verify `Flush()` returns `PointData` and the points appear in the output

### DecodeError Counter / Resolver Lookup
- Unit tests for each; resolver test verifies dictionary lookup matches old linear search

## Out of Scope

- `byte[]` payload pooling in Reader (8-byte allocations are cheap on young-gen GC; BitArray elimination covers the bigger win)
- Progress reporting / resumability
- Per-file error recovery beyond try/catch in handler
- InfluxDB write parallelism tuning

## Impact Assessment

| Change | Expected Impact | Risk |
|--------|----------------|------|
| Pipeline reuse | High — eliminates block recreation, improves batching | Low — structural refactor only |
| BitArray elimination | High — removes largest hot-path allocation | Medium — touches shared Decoder.cs API |
| Async Reader | Medium — unblocks thread pool during I/O | Low — additive, keep sync path |
| DecodeError counter | Low — minor GC reduction | Low — simple replacement |
| Flush() bug fix | Correctness — data no longer silently lost | Low — straightforward |
| Resolver lookup | Low — only matters with many frame rules | Low — internal change |