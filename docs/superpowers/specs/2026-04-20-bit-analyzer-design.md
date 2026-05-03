# Bit Analyzer — Design Spec

## Goal

A visual debugging tool for discovering signal boundaries in CAN bus data by analyzing bit-flip patterns between consecutive frames. Works with both loaded log files (with playback) and live Panda connections.

## Route & Navigation

- New route: `/bits`
- "Bits" nav link in top AppBar (between Editor and Frames)
- Requires either a loaded log file or live Panda connection

## Page Layout

Hybrid layout: left panel (280px) + right panel (flex).

### Left Panel

1. **Frame ID selector** — dropdown showing distinct frame IDs from loaded data
2. **Mux toggle** — filter frames by multiplex switch value (same ToggleButtonGroup pattern as existing BitGrid)
3. **Playback controls** — slider + play/pause + speed (0.5x, 1x, 2x, 5x)
   - Slider range: first to last frame index for the selected frame ID
   - Play advances to next frame at the selected speed
   - Only visible in log mode (not live)

### Right Panel — Always Visible

**Bit Flip Diff Grid** and **Flip Heatmap Grid** side-by-side (or stacked on narrow screens):

- **Bit Flip Diff**: 8-bit-per-row grid matching BitGrid bits-view layout. Green cells for 0→1 transitions, red for 1→0, dark gray for no change. Shows consecutive frames frame[N-1] → frame[N]. Legend below grid.
- **Flip Heatmap**: Same 8-bit-per-row grid. Cell intensity proportional to total flip count across all frames in the analysis window. Green scale (darker = fewer flips, brighter = more flips). Count label in each cell.

### Right Panel — Expandable Sections

1. **Stability & Entropy**
   - Horizontal bars per bit position
   - Stability: fraction of frames where bit is unchanged (blue scale)
   - Entropy: Shannon entropy per bit (orange scale)
   - Low stability/entropy = padding/reserved bits
   - High entropy = data-carrying signal bits

2. **Change Correlation**
   - NxN matrix where N = message_size × 8 bits
   - Cell (i,j) = Pearson correlation of flip events between bit i and bit j
   - Green = positively correlated (likely same multi-bit signal)
   - Dark = uncorrelated
   - Click a cell to highlight the corresponding bit pair in the diff grid

3. **Value Clustering**
   - Per-byte histogram of unique values observed
   - Shows distinct value count per byte position
   - Patterns like values at multiples of 64 reveal 2-bit signals in top bits

## Data Sources

### Log Mode
- New backend endpoint: `GET /api/log/raw-frames?frameId=X&skip=Y&take=Z`
- Returns `[{ frameTime, frameId, busId, payload: number[] }]`
- Frames are pre-filtered by frame ID on the server
- Pagination with skip/take for large logs

### Live Mode
- WebSocket already sends `rawData` via `WsRawFrameMessage`
- Accumulate raw frames in store alongside decoded signal data
- Default window: last 100 frames per tracked frame ID
- Older frames pruned automatically

## Analysis Computation

All analysis computed client-side for maximum interactivity:

- **Flip count**: Increment counter per bit when bit differs from previous frame of same ID
- **Stability**: `1 - (flip_count / total_frame_count)`
- **Shannon entropy**: `-p(0)*log2(p(0)) - p(1)*log2(p(1))` per bit
- **Correlation**: Pearson correlation of bit-flip boolean vectors between two bit positions
- **Value clustering**: Count distinct values per byte, bucket by common factors

## Backend Changes

### New Endpoint

```
GET /api/log/raw-frames?frameId=X&skip=Y&take=Z
```

Returns raw CAN frames filtered by frame ID with pagination. Uses `CanLogService.GetFramesAsync` with the existing `frameId` filter. Returns only the fields needed for bit analysis: `frameTime`, `frameId`, `busId`, `payload`.

## Frontend Components

1. **BitAnalyzerPage** — Page component, route `/bits`, layout orchestrator
2. **BitFlipGrid** — Diff grid showing green/red/gray per bit for consecutive frame pair
3. **FlipHeatmap** — Intensity grid showing flip frequency per bit across window
4. **PlaybackControls** — Slider, play/pause button, speed selector
5. **StabilityEntropy** — Expandable section with horizontal bar charts
6. **CorrelationMatrix** — Expandable NxN grid with click-to-highlight
7. **ValueClustering** — Expandable section with per-byte histograms

## Store Changes

Add to `AppState`:
```typescript
analysisFrames: Map<number, RawFrame[]>  // Raw frames per frame ID
analysisWindow: number                    // Configurable, default 100
```

New actions:
```typescript
loadAnalysisFrames(frameId: number): Promise<void>
clearAnalysisData(): void
addLiveAnalysisFrame(frame: WsRawFrameMessage): void
```

## Decisions

- Adjacent consecutive comparison only (no arbitrary frame pairs)
- Grid rendering for diff and heatmap (consistent with existing BitGrid)
- Client-side computation for all analysis methods
- Mux toggle as top-level control (same pattern as BitGrid)
- Playback with play/pause and speed for log mode only