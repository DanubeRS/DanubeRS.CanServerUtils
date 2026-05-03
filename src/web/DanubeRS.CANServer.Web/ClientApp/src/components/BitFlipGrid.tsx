import { useMemo } from 'react';
import { Box, Typography, Stack } from '@mui/material';
import { useDbcStore } from '../store';
import type { RawFrameInfo, SignalInfo } from '../api';

interface BitFlipGridProps {
  frames: RawFrameInfo[];
  highlightBits?: number[];
}

const SIGNAL_COLORS = [
  { bg: 'rgba(74,158,255,0.12)', border: 'rgba(74,158,255,0.3)' },
  { bg: 'rgba(168,85,247,0.12)', border: 'rgba(168,85,247,0.3)' },
  { bg: 'rgba(34,197,94,0.12)', border: 'rgba(34,197,94,0.3)' },
  { bg: 'rgba(251,191,36,0.12)', border: 'rgba(251,191,36,0.3)' },
  { bg: 'rgba(236,72,153,0.12)', border: 'rgba(236,72,153,0.3)' },
  { bg: 'rgba(14,165,233,0.12)', border: 'rgba(14,165,233,0.3)' },
  { bg: 'rgba(249,115,22,0.12)', border: 'rgba(249,115,22,0.3)' },
  { bg: 'rgba(132,204,22,0.12)', border: 'rgba(132,204,22,0.3)' },
];

function buildBitOwnership(signals: SignalInfo[], totalBits: number): Map<number, { signal: SignalInfo; colorIdx: number }> {
  const map = new Map<number, { signal: SignalInfo; colorIdx: number }>();
  let colorIdx = 0;
  for (const sig of signals) {
    const idx = colorIdx++;
    for (let i = sig.startBit; i < sig.startBit + sig.size && i < totalBits; i++) {
      if (!map.has(i)) map.set(i, { signal: sig, colorIdx: idx % SIGNAL_COLORS.length });
    }
  }
  return map;
}

export function BitFlipGrid({ frames, highlightBits = [] }: BitFlipGridProps) {
  const playbackIndex = useDbcStore(s => s.analysisPlaybackIndex);
  const selectedFrameId = useDbcStore(s => s.selectedFrameId);
  const messages = useDbcStore(s => s.messages);

  const message = selectedFrameId !== null ? messages.find(m => m.frameId === selectedFrameId) : null;

  const diff = useMemo(() => {
    if (frames.length === 0) return null;
    const currentPayload = frames[playbackIndex]?.payload;
    if (!currentPayload) return null;
    if (playbackIndex === 0) return { current: currentPayload, prev: null };
    const prevPayload = frames[playbackIndex - 1]?.payload;
    return { current: currentPayload, prev: prevPayload ?? null };
  }, [frames, playbackIndex]);

  if (frames.length === 0) {
    return <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>No data — select a frame ID</Typography>;
  }

  if (!diff) return null;

  const { current, prev } = diff;
  const totalBits = current.length * 8;
  const bitOwnership = message ? buildBitOwnership(message.signals, totalBits) : null;

  const rows: React.ReactNode[] = [];
  for (let byte = 0; byte < current.length; byte++) {
    const cells: React.ReactNode[] = [];
    for (let bit = 0; bit < 8; bit++) {
      const globalBit = byte * 8 + bit;
      const byteVal = current[byte];
      const currentBit = (byteVal >> bit) & 1;
      const prevBit = prev ? ((prev[byte] >> bit) & 1) : currentBit;

      let bg: string;
      let label: string;
      if (!prev) {
        bg = 'rgba(128,128,128,0.2)';
        label = String(currentBit);
      } else if (prevBit === 0 && currentBit === 1) {
        bg = 'rgba(76,175,80,0.35)';
        label = '1';
      } else if (prevBit === 1 && currentBit === 0) {
        bg = 'rgba(244,67,54,0.35)';
        label = '0';
      } else {
        bg = 'rgba(128,128,128,0.1)';
        label = String(currentBit);
      }

      const highlighted = highlightBits.includes(globalBit);
      const owner = bitOwnership?.get(globalBit);
      const signalBorder = owner ? `1px solid ${SIGNAL_COLORS[owner.colorIdx].border}` : '1px solid rgba(128,128,128,0.2)';
      const signalBg = owner && !prev ? SIGNAL_COLORS[owner.colorIdx].bg : bg;

      cells.push(
        <Box
          key={globalBit}
          sx={{
            width: 28, height: 28, display: 'flex', alignItems: 'center', justifyContent: 'center',
            borderRadius: 0.5, bgcolor: prev ? bg : signalBg,
            border: highlighted ? '2px solid #ff9f43' : signalBorder,
            fontSize: '0.65rem', fontFamily: 'monospace', color: 'text.primary',
            ml: bit === 0 ? 0 : '2px',
          }}
          title={owner ? `${owner.signal.name} [${owner.signal.startBit}:${owner.signal.startBit + owner.signal.size - 1}] — Bit ${globalBit}: ${prev !== null ? `${prevBit}→${currentBit}` : currentBit}` : `Bit ${globalBit}: ${prev !== null ? `${prevBit}→${currentBit}` : currentBit}`}
        >
          {label}
        </Box>
      );
    }
    rows.push(
      <Stack key={byte} direction="row" spacing={0} sx={{ alignItems: 'center', gap: '2px' }}>
        <Typography variant="caption" sx={{ width: 24, fontSize: '0.6rem', color: 'text.disabled', fontFamily: 'monospace' }}>B{byte}</Typography>
        {cells}
      </Stack>
    );
  }

  const uniqueSignals = message ? [...new Set(message.signals.map(s => s.name))] : [];

  return (
    <Box sx={{ mb: 3 }}>
      <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
        Bit Flip Diff {prev !== null ? `(frame ${playbackIndex - 1} → ${playbackIndex})` : `(frame ${playbackIndex})`}
      </Typography>
      <Stack spacing={0.25}>{rows}</Stack>
      <Stack direction="row" spacing={1.5} sx={{ mt: 0.5, flexWrap: 'wrap' }}>
        <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
          <Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(76,175,80,0.35)', border: '1px solid rgba(76,175,80,0.5)' }} />
          <Typography variant="caption">0→1</Typography>
        </Stack>
        <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
          <Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(244,67,54,0.35)', border: '1px solid rgba(244,67,54,0.5)' }} />
          <Typography variant="caption">1→0</Typography>
        </Stack>
        <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
          <Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(128,128,128,0.1)', border: '1px solid rgba(128,128,128,0.2)' }} />
          <Typography variant="caption">No change</Typography>
        </Stack>
        {bitOwnership && uniqueSignals.length > 0 && (
          <Box sx={{ display: 'flex', gap: 0.5, flexWrap: 'wrap', ml: 1, pl: 1, borderLeft: '1px solid rgba(128,128,128,0.3)' }}>
            {uniqueSignals.map((name, i) => (
              <Stack key={name} direction="row" spacing={0.25} sx={{ alignItems: 'center' }}>
                <Box sx={{ width: 10, height: 10, borderRadius: 0.25, bgcolor: SIGNAL_COLORS[i % SIGNAL_COLORS.length].bg, border: `1px solid ${SIGNAL_COLORS[i % SIGNAL_COLORS.length].border}` }} />
                <Typography variant="caption" sx={{ fontSize: '0.6rem' }}>{name}</Typography>
              </Stack>
            ))}
          </Box>
        )}
      </Stack>
    </Box>
  );
}