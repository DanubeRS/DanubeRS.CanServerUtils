import { useMemo, useState } from 'react';
import { Box, Typography, Collapse, IconButton, Stack } from '@mui/material';
import { ExpandMore, ExpandLess } from '@mui/icons-material';
import type { RawFrameInfo } from '../api';

interface CorrelationMatrixProps {
  frames: RawFrameInfo[];
  onHighlightBits?: (bits: number[]) => void;
}

export function CorrelationMatrix({ frames, onHighlightBits }: CorrelationMatrixProps) {
  const [open, setOpen] = useState(false);

  const matrix = useMemo(() => {
    if (frames.length < 3) return null;
    const first = frames[0].payload;
    const totalBits = first.length * 8;
    const flipEvents: boolean[][] = [];

    for (let i = 1; i < frames.length; i++) {
      const prev = frames[i - 1].payload;
      const cur = frames[i].payload;
      const events: boolean[] = new Array(totalBits).fill(false);
      for (let b = 0; b < first.length; b++) {
        const xor = prev[b] ^ cur[b];
        for (let bit = 0; bit < 8; bit++) {
          if ((xor >> bit) & 1) events[b * 8 + bit] = true;
        }
      }
      flipEvents.push(events);
    }

    const n = flipEvents.length;
    if (n === 0) return null;

    const mean = (arr: boolean[]) => arr.filter(Boolean).length / n;
    const means = new Array(totalBits).fill(0);
    for (let i = 0; i < totalBits; i++) means[i] = mean(flipEvents.map(e => e[i]));

    const corr: number[][] = [];
    for (let i = 0; i < totalBits; i++) {
      corr[i] = new Array(totalBits).fill(0);
      for (let j = 0; j < totalBits; j++) {
        if (i === j) { corr[i][j] = 1; continue; }
        const mi = means[i], mj = means[j];
        if (mi === 0 || mj === 0 || mi === 1 || mj === 1) { corr[i][j] = 0; continue; }
        let num = 0, di = 0, dj = 0;
        for (let k = 0; k < n; k++) {
          const xi = flipEvents[k][i] ? 1 : 0;
          const xj = flipEvents[k][j] ? 1 : 0;
          num += (xi - mi) * (xj - mj);
          di += (xi - mi) ** 2;
          dj += (xj - mj) ** 2;
        }
        corr[i][j] = (di > 0 && dj > 0) ? num / Math.sqrt(di * dj) : 0;
      }
    }

    return { corr, totalBits };
  }, [frames]);

  if (frames.length < 3) return null;

  const cellSize = matrix && matrix.totalBits <= 64 ? 12 : 8;

  return (
    <Box sx={{ mb: 2, border: 1, borderColor: 'divider', borderRadius: 1 }}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', p: 1, cursor: 'pointer' }} onClick={() => setOpen(!open)}>
        <Typography variant="subtitle2">Change Correlation</Typography>
        <Typography variant="caption" color="text.secondary">(need {Math.max(0, 3 - frames.length)} more frames)</Typography>
        <IconButton size="small">{open ? <ExpandLess /> : <ExpandMore />}</IconButton>
      </Stack>
      <Collapse in={open}>
        <Box sx={{ px: 1, pb: 1, overflow: 'auto' }}>
          {matrix && (
            <Box sx={{ display: 'inline-block' }}>
              <Box sx={{ display: 'grid', gridTemplateColumns: `repeat(${matrix.totalBits}, ${cellSize}px)`, gap: '1px' }}>
                {Array.from({ length: matrix.totalBits * matrix.totalBits }, (_, idx) => {
                  const i = Math.floor(idx / matrix.totalBits);
                  const j = idx % matrix.totalBits;
                  const v = matrix.corr[i][j];
                  const isDiag = i === j;
                  const bg = v > 0
                    ? `rgba(76,175,80,${0.05 + Math.abs(v) * 0.5})`
                    : v < 0
                    ? `rgba(244,67,54,${0.05 + Math.abs(v) * 0.5})`
                    : 'rgba(128,128,128,0.05)';
                  return (
                    <Box
                      key={idx}
                      sx={{ width: cellSize, height: cellSize, bgcolor: isDiag ? 'rgba(33,150,243,0.15)' : bg, cursor: 'pointer', '&:hover': { outline: '1px solid white' } }}
                      onClick={() => onHighlightBits?.([i, j])}
                      title={`${i}↔${j}: ${v.toFixed(2)}`}
                    />
                  );
                })}
              </Box>
            </Box>
          )}
        </Box>
      </Collapse>
    </Box>
  );
}