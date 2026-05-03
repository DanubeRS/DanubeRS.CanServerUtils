import { useState, useRef, useMemo, useCallback } from 'react';
import { Box, Typography, ToggleButtonGroup, ToggleButton, Button } from '@mui/material';
import { UploadFile } from '@mui/icons-material';
import { useDbcStore } from '../store';
import { FrameSelector } from '../components/FrameSelector';
import { PlaybackControls } from '../components/PlaybackControls';
import { BitFlipGrid } from '../components/BitFlipGrid';
import { FlipHeatmap } from '../components/FlipHeatmap';
import { StabilityEntropy } from '../components/StabilityEntropy';
import { CorrelationMatrix } from '../components/CorrelationMatrix';
import { ValueClustering } from '../components/ValueClustering';
import type { RawFrameInfo } from '../api';

const EMPTY_FRAMES: RawFrameInfo[] = [];

function extractSignalValue(payload: number[], startBit: number, size: number, order: string): number {
  const totalBits = payload.length * 8;
  let value = 0;
  for (let i = 0; i < size; i++) {
    let bitPos: number;
    if (order === 'Motorola' || order === 'big_endian') {
      bitPos = startBit + i;
    } else {
      bitPos = startBit + i;
    }
    if (bitPos >= totalBits) continue;
    const byteIdx = Math.floor(bitPos / 8);
    const bitIdx = bitPos % 8;
    if ((payload[byteIdx] >> bitIdx) & 1) {
      value |= (1 << i);
    }
  }
  return value;
}

export default function BitAnalyzerPage() {
  const logLoaded = useDbcStore(s => s.logLoaded);
  const pandasConnected = useDbcStore(s => s.pandasConnected);
  const loaded = useDbcStore(s => s.loaded);
  const loading = useDbcStore(s => s.loading);
  const loadDbc = useDbcStore(s => s.loadDbc);
  const selectedFrameId = useDbcStore(s => s.selectedFrameId);
  const messages = useDbcStore(s => s.messages);
  const analysisFrames = useDbcStore(s => s.analysisFrames);
  const liveFrameHistory = useDbcStore(s => s.liveFrameHistory);
  const liveFrameHistoryVersion = useDbcStore(s => s.liveFrameHistoryVersion);
  const [highlightBits, setHighlightBits] = useState<number[]>([]);
  const [muxFilter, setMuxFilter] = useState<number | null>(null);
  const dbcFileRef = useRef<HTMLInputElement>(null);

  const handleDbcUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) await loadDbc(file);
    e.target.value = '';
  };

  const selectedAnalysisFrames = useMemo(() => {
    if (selectedFrameId === null) return undefined;
    return analysisFrames.get(selectedFrameId);
  }, [selectedFrameId, analysisFrames]);

  const selectedLiveHistory = useMemo(() => {
    if (!pandasConnected || selectedFrameId === null) return EMPTY_FRAMES;
    return liveFrameHistory.get(selectedFrameId) ?? EMPTY_FRAMES;
  }, [pandasConnected, selectedFrameId, liveFrameHistory, liveFrameHistoryVersion]);

  const rawFrames = pandasConnected ? selectedLiveHistory : (selectedAnalysisFrames ?? EMPTY_FRAMES);

  const muxSwitchValues = useMemo(() => {
    if (selectedFrameId === null) return [];
    const msg = messages.find(m => m.frameId === selectedFrameId);
    if (!msg) return [];
    return [...new Set(
      msg.signals
        .filter(s => s.multiplex?.switchValue != null)
        .map(s => s.multiplex!.switchValue!)
    )].sort((a, b) => a - b);
  }, [selectedFrameId, messages]);

  const muxSignal = useMemo(() => {
    if (selectedFrameId === null) return null;
    const msg = messages.find(m => m.frameId === selectedFrameId);
    if (!msg) return null;
    return msg.signals.find(s => s.multiplex?.isSwitch) ?? null;
  }, [selectedFrameId, messages]);

  const filteredFrames = useMemo(() => {
    if (muxFilter === null || !muxSignal) return rawFrames;
    return rawFrames.filter(f => {
      const muxValue = extractSignalValue(f.payload, muxSignal.startBit, muxSignal.size, muxSignal.order);
      return muxValue === muxFilter;
    });
  }, [rawFrames, muxFilter, muxSignal]);

  const handleMuxChange = useCallback((_: React.MouseEvent<HTMLElement>, value: string | null) => {
    if (value === null || value === 'all') {
      setMuxFilter(null);
    } else {
      setMuxFilter(Number(value));
    }
  }, []);

  if (!logLoaded && !pandasConnected) {
    return (
      <Box sx={{ p: 2 }}>
        <Typography variant="body2" color="text.secondary">
          Load a log file or connect to a Panda to use bit analysis.
        </Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ display: 'flex', height: '100%', overflow: 'hidden' }}>
      <Box sx={{ width: 280, borderRight: 1, borderColor: 'divider', p: 2, overflowY: 'auto', flexShrink: 0 }}>
        <Typography variant="subtitle2" sx={{ mb: 1 }}>Frame Selection</Typography>
        {!loaded && (
          <Box sx={{ mb: 1 }}>
            <input ref={dbcFileRef} type="file" accept=".dbc" onChange={handleDbcUpload} hidden />
            <Button variant="outlined" size="small" startIcon={<UploadFile fontSize="small" />} onClick={() => dbcFileRef.current?.click()} disabled={loading} fullWidth>
              {loading ? 'Loading...' : 'Load DBC for names'}
            </Button>
          </Box>
        )}
        {loaded && (
          <Typography variant="caption" sx={{ display: 'block', mb: 1, color: 'text.secondary' }}>
            ✓ DBC loaded ({messages.length} messages)
          </Typography>
        )}
        <FrameSelector />
        {muxSwitchValues.length > 0 && (
          <Box sx={{ mt: 2 }}>
            <Typography variant="caption" sx={{ color: 'text.secondary', mr: 0.5 }}>Mux:</Typography>
            <ToggleButtonGroup
              size="small"
              exclusive
              value={muxFilter === null ? 'all' : String(muxFilter)}
              onChange={handleMuxChange}
            >
              <ToggleButton value="all">All</ToggleButton>
              {muxSwitchValues.map(sv => (
                <ToggleButton key={sv} value={String(sv)}>m{sv}</ToggleButton>
              ))}
            </ToggleButtonGroup>
          </Box>
        )}
        {(logLoaded || pandasConnected) && selectedFrameId !== null && <PlaybackControls frames={filteredFrames} liveMode={pandasConnected} />}
      </Box>
      <Box sx={{ flexGrow: 1, p: 2, overflowY: 'auto' }}>
        <BitFlipGrid frames={filteredFrames} highlightBits={highlightBits} />
        <FlipHeatmap frames={filteredFrames} />
        <StabilityEntropy frames={filteredFrames} />
        <CorrelationMatrix frames={filteredFrames} onHighlightBits={setHighlightBits} />
        <ValueClustering frames={filteredFrames} />
      </Box>
    </Box>
  );
}