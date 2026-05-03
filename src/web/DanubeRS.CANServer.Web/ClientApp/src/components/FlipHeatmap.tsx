import { useMemo } from 'react';
import { Box, Typography, Stack } from '@mui/material';
import { useDbcStore } from '../store';
import type { RawFrameInfo, SignalInfo } from '../api';

interface FlipHeatmapProps {
  frames: RawFrameInfo[];
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

export function FlipHeatmap({ frames }: FlipHeatmapProps) {
  const selectedFrameId = useDbcStore(s => s.selectedFrameId);
  const messages = useDbcStore(s => s.messages);
  const message = selectedFrameId !== null ? messages.find(m => m.frameId === selectedFrameId) : null;

  const flipCounts = useMemo(() => {
    if (frames.length < 2) return null;
    const first = frames[0].payload;
    const totalBits = first.length * 8;
    const counts = new Array(totalBits).fill(0);
    for (let i = 1; i < frames.length; i++) {
      const prev = frames[i - 1].payload;
      const cur = frames[i].payload;
      for (let b = 0; b < first.length; b++) {
        const xor = prev[b] ^ cur[b];
        for (let bit = 0; bit < 8; bit++) {
          if ((xor >> bit) & 1) counts[b * 8 + bit]++;
        }
      }
    }
    return { counts, totalBits, maxCount: Math.max(...counts, 1), byteCount: first.length };
  }, [frames]);

  if (frames.length < 2) {
    return <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>Need at least 2 frames for heatmap</Typography>;
  }

  if (!flipCounts) return null;
  const { counts, maxCount, byteCount } = flipCounts;
  const totalBits = flipCounts.totalBits;
  const bitOwnership = message ? buildBitOwnership(message.signals, totalBits) : null;

  const rows: React.ReactNode[] = [];
  for (let byte = 0; byte < byteCount; byte++) {
    const cells: React.ReactNode[] = [];
    for (let bit = 0; bit < 8; bit++) {
      const globalBit = byte * 8 + bit;
      const count = counts[globalBit];
      const intensity = count / maxCount;
      const owner = bitOwnership?.get(globalBit);

      const bg = owner
        ? `rgba(${74 + owner.colorIdx * 25 % 180},${175 - Math.round(intensity * 50)},${80 + owner.colorIdx * 35 % 175},${0.08 + intensity * 0.42})`
        : `rgba(76,${175 - Math.round(intensity * 50)},80,${0.08 + intensity * 0.42})`;
      const border = owner ? `1px solid ${SIGNAL_COLORS[owner.colorIdx].border}` : '1px solid rgba(76,175,80,0.15)';

      cells.push(
        <Box
          key={globalBit}
          sx={{
            width: 28, height: 28, display: 'flex', alignItems: 'center', justifyContent: 'center',
            borderRadius: 0.5, bgcolor: bg,
            border,
            fontSize: '0.6rem', fontFamily: 'monospace', color: 'text.primary',
            ml: bit === 0 ? 0 : '2px',
          }}
          title={owner ? `${owner.signal.name} — Bit ${globalBit}: ${count} flips` : `Bit ${globalBit}: ${count} flips`}
        >
          {count}
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

  return (
    <Box sx={{ mb: 3 }}>
      <Typography variant="subtitle2" sx={{ mb: 0.5 }}>Flip Heatmap ({frames.length - 1} pairs)</Typography>
      <Stack spacing={0.25}>{rows}</Stack>
    </Box>
  );
}