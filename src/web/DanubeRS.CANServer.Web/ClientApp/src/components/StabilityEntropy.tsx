import { useMemo, useState } from 'react';
import { Box, Typography, Collapse, IconButton, Stack } from '@mui/material';
import { ExpandMore, ExpandLess } from '@mui/icons-material';
import type { RawFrameInfo } from '../api';

interface StabilityEntropyProps {
  frames: RawFrameInfo[];
}

export function StabilityEntropy({ frames }: StabilityEntropyProps) {
  const [open, setOpen] = useState(false);

  const stats = useMemo(() => {
    if (frames.length < 2) return null;
    const first = frames[0].payload;
    const totalBits = first.length * 8;
    const flipCounts = new Array(totalBits).fill(0);
    const zeroCounts = new Array(totalBits).fill(0);
    const totalPairs = frames.length - 1;

    for (let i = 0; i < frames.length; i++) {
      const p = frames[i].payload;
      for (let b = 0; b < first.length; b++) {
        for (let bit = 0; bit < 8; bit++) {
          const globalBit = b * 8 + bit;
          if (!((p[b] >> bit) & 1)) {
            zeroCounts[globalBit]++;
          }
        }
      }
    }

    for (let i = 1; i < frames.length; i++) {
      const prev = frames[i - 1].payload;
      const cur = frames[i].payload;
      for (let b = 0; b < first.length; b++) {
        const xor = prev[b] ^ cur[b];
        for (let bit = 0; bit < 8; bit++) {
          if ((xor >> bit) & 1) flipCounts[b * 8 + bit]++;
        }
      }
    }

    const stability = flipCounts.map(c => 1 - c / totalPairs);
    const entropy = zeroCounts.map((z, _i) => {
      const p0 = z / frames.length;
      const p1 = 1 - p0;
      if (p0 === 0 || p1 === 0) return 0;
      return -(p0 * Math.log2(p0)) - (p1 * Math.log2(p1));
    });

    return { stability, entropy, totalBits, byteCount: first.length };
  }, [frames]);

  if (frames.length < 2) return null;

  return (
    <Box sx={{ mb: 2, border: 1, borderColor: 'divider', borderRadius: 1 }}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', p: 1, cursor: 'pointer' }} onClick={() => setOpen(!open)}>
        <Typography variant="subtitle2">Stability & Entropy</Typography>
        <IconButton size="small">{open ? <ExpandLess /> : <ExpandMore />}</IconButton>
      </Stack>
      <Collapse in={open}>
        {stats && (
          <Box sx={{ px: 1, pb: 1 }}>
            {Array.from({ length: stats.totalBits }, (_, globalBit) => {
              const byte = Math.floor(globalBit / 8);
              const bit = globalBit % 8;
              const s = stats.stability[globalBit];
              const e = stats.entropy[globalBit];
              return (
                <Stack key={globalBit} direction="row" spacing={1} sx={{ alignItems: 'center', mb: 0.25 }}>
                  <Typography variant="caption" sx={{ width: 36, fontSize: '0.6rem', fontFamily: 'monospace', color: 'text.disabled' }}>
                    B{byte}.{bit}
                  </Typography>
                  <Box sx={{ flexGrow: 1, display: 'flex', gap: 0.5 }}>
                    <Box sx={{ flex: 1, position: 'relative', height: 14, borderRadius: 0.5, bgcolor: 'rgba(33,150,243,0.1)', overflow: 'hidden' }}>
                      <Box sx={{ position: 'absolute', left: 0, top: 0, bottom: 0, width: `${s * 100}%`, bgcolor: `rgba(33,150,243,${0.2 + s * 0.5})`, borderRadius: 0.5 }} />
                      <Typography variant="caption" sx={{ position: 'absolute', left: 4, top: 0, fontSize: '0.5rem', lineHeight: '14px', color: 'text.primary', fontFamily: 'monospace' }}>
                        {s.toFixed(2)}
                      </Typography>
                    </Box>
                    <Box sx={{ flex: 1, position: 'relative', height: 14, borderRadius: 0.5, bgcolor: 'rgba(255,152,0,0.1)', overflow: 'hidden' }}>
                      <Box sx={{ position: 'absolute', left: 0, top: 0, bottom: 0, width: `${e * 100}%`, bgcolor: `rgba(255,152,0,${0.2 + e * 0.5})`, borderRadius: 0.5 }} />
                      <Typography variant="caption" sx={{ position: 'absolute', left: 4, top: 0, fontSize: '0.5rem', lineHeight: '14px', color: 'text.primary', fontFamily: 'monospace' }}>
                        {e.toFixed(2)}
                      </Typography>
                    </Box>
                  </Box>
                </Stack>
              );
            })}
            <Stack direction="row" spacing={2} sx={{ mt: 0.5 }}>
              <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
                <Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(33,150,243,0.5)' }} />
                <Typography variant="caption">Stability</Typography>
              </Stack>
              <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
                <Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(255,152,0,0.5)' }} />
                <Typography variant="caption">Entropy</Typography>
              </Stack>
            </Stack>
          </Box>
        )}
      </Collapse>
    </Box>
  );
}