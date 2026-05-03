import { useMemo, useState } from 'react';
import { Box, Typography, Collapse, IconButton, Stack } from '@mui/material';
import { ExpandMore, ExpandLess } from '@mui/icons-material';
import type { RawFrameInfo } from '../api';

interface ValueClusteringProps {
  frames: RawFrameInfo[];
}

export function ValueClustering({ frames }: ValueClusteringProps) {
  const [open, setOpen] = useState(false);

  const histograms = useMemo(() => {
    if (frames.length === 0) return null;
    const first = frames[0].payload;
    const result: { byte: number; uniqueCount: number; histogram: Map<number, number> }[] = [];

    for (let b = 0; b < first.length; b++) {
      const counts = new Map<number, number>();
      for (const frame of frames) {
        const val = frame.payload[b] ?? 0;
        counts.set(val, (counts.get(val) ?? 0) + 1);
      }
      result.push({ byte: b, uniqueCount: counts.size, histogram: counts });
    }
    return result;
  }, [frames]);

  if (frames.length === 0) return null;

  return (
    <Box sx={{ mb: 2, border: 1, borderColor: 'divider', borderRadius: 1 }}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', p: 1, cursor: 'pointer' }} onClick={() => setOpen(!open)}>
        <Typography variant="subtitle2">Value Clustering</Typography>
        <IconButton size="small">{open ? <ExpandLess /> : <ExpandMore />}</IconButton>
      </Stack>
      <Collapse in={open}>
        <Box sx={{ px: 1, pb: 1 }}>
          {histograms && histograms.map(({ byte, uniqueCount, histogram }) => {
            const entries = [...histogram.entries()].sort((a, b) => a[0] - b[0]);
            const maxCount = Math.max(...histogram.values(), 1);
            return (
              <Box key={byte} sx={{ mb: 1 }}>
                <Typography variant="caption" sx={{ fontFamily: 'monospace', color: 'text.secondary' }}>
                  B{byte} — {uniqueCount} unique values
                </Typography>
                <Box sx={{ display: 'flex', gap: '1px', height: 24, alignItems: 'flex-end', mt: 0.25 }}>
                  {entries.slice(0, 32).map(([val, count]) => (
                    <Box
                      key={val}
                      sx={{
                        width: Math.max(4, Math.floor(200 / Math.min(entries.length, 32))),
                        height: Math.max(2, (count / maxCount) * 24),
                        bgcolor: 'rgba(76,175,80,0.6)',
                        borderRadius: 0.25,
                        minWidth: 2,
                      }}
                      title={`0x${val.toString(16).padStart(2, '0')}: ${count}`}
                    />
                  ))}
                  {entries.length > 32 && (
                    <Typography variant="caption" sx={{ ml: 0.5, color: 'text.disabled', alignSelf: 'center' }}>
                      +{entries.length - 32} more
                    </Typography>
                  )}
                </Box>
              </Box>
            );
          })}
        </Box>
      </Collapse>
    </Box>
  );
}