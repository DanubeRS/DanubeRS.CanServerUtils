import { useState, useMemo } from 'react';
import { Box, Typography, Stack, ToggleButtonGroup, ToggleButton } from '@mui/material';
import { useDbcStore } from '../store';

interface BitGridProps {
  frameId: number;
  messageSize: number;
}

export function BitGrid({ frameId, messageSize }: BitGridProps) {
  const messages = useDbcStore(s => s.messages);
  const selectSignal = useDbcStore(s => s.selectSignal);
  const selectedSignal = useDbcStore(s => s.selectedSignal);
  const [viewMode, setViewMode] = useState<'bits' | 'blocks'>('bits');
  const [muxFilter, setMuxFilter] = useState<number | null>(null);

  const message = messages.find(m => m.frameId === frameId);
  if (!message) return null;

  const muxColors = [
    { bg: 'rgba(168,85,247,0.2)', border: '#a855f7' },
    { bg: 'rgba(34,197,94,0.2)', border: '#22c55e' },
    { bg: 'rgba(251,191,36,0.2)', border: '#fbbf24' },
    { bg: 'rgba(236,72,153,0.2)', border: '#ec4899' },
    { bg: 'rgba(14,165,233,0.2)', border: '#0ea5e9' },
    { bg: 'rgba(249,115,22,0.2)', border: '#f97316' },
  ];

  const hasMultiplex = message.signals.some(s => s.multiplex !== null);

  const muxSwitchValues = [...new Set(
    message.signals
      .filter(s => s.multiplex?.switchValue != null)
      .map(s => s.multiplex!.switchValue!)
  )].sort((a, b) => a - b);

  const activeMuxValue = muxFilter ?? (selectedSignal
    ? (message.signals.find(s => s.name === selectedSignal.signalName)?.multiplex?.switchValue ?? null)
    : null);

  const visibleSignals = hasMultiplex
    ? message.signals.filter(sig => {
        if (!sig.multiplex) return true;
        if (sig.multiplex.isSwitch) return true;
        if (activeMuxValue === null) return true;
        return sig.multiplex.switchValue === activeMuxValue;
      })
    : message.signals;

  const muxSwitchValuesMap = new Map<string, number | null>();
  for (const sig of visibleSignals) {
    if (sig.multiplex?.isSwitch) muxSwitchValuesMap.set(sig.name, null);
    else if (sig.multiplex?.switchValue != null) muxSwitchValuesMap.set(sig.name, sig.multiplex.switchValue);
  }

  const totalBits = messageSize * 8;
  const isSelected = (sigName: string) => selectedSignal?.frameId === frameId && selectedSignal?.signalName === sigName;

  const getSignalStyle = (sig: typeof visibleSignals[0]) => {
    const muxValue = sig.multiplex?.switchValue ?? null;
    const isMuxSwitch = sig.multiplex?.isSwitch ?? false;
    const muxColorIdx = muxValue !== null ? muxValue % muxColors.length : -1;
    if (isSelected(sig.name)) return { bg: 'rgba(233,69,96,0.18)', border: '#e94560' };
    if (isMuxSwitch) return { bg: 'rgba(168,85,247,0.3)', border: '#a855f7' };
    if (muxColorIdx >= 0) return { bg: muxColors[muxColorIdx].bg, border: muxColors[muxColorIdx].border };
    return { bg: 'rgba(74,158,255,0.15)', border: 'rgba(74,158,255,0.3)' };
  };

  // For each bit, compute which signal owns it (or null)
  const bitOwners = useMemo(() => {
    const map = new Map<number, typeof visibleSignals[0] | null>();
    for (const sig of visibleSignals) {
      for (let i = sig.startBit; i < sig.startBit + sig.size && i < totalBits; i++) {
        if (!map.has(i)) map.set(i, sig);
      }
    }
    return map;
  }, [visibleSignals, totalBits]);

  // For the selected signal, compute contiguous runs per byte row
  // Each run: { byte, startBitInRow, endBitInRow } (bit positions 0-7 within the byte)
  const selectedRuns = useMemo(() => {
    if (!selectedSignal) return [];
    const sig = visibleSignals.find(s => isSelected(s.name));
    if (!sig) return [];
    const runs: { byte: number; start: number; end: number }[] = [];
    const sigStart = sig.startBit;
    const sigEnd = sigStart + sig.size - 1;
    for (let byte = 0; byte < messageSize; byte++) {
      const rowStart = byte * 8;
      const rowEnd = rowStart + 7;
      if (sigEnd < rowStart || sigStart > rowEnd) continue;
      const runStart = Math.max(sigStart, rowStart) - rowStart;
      const runEnd = Math.min(sigEnd, rowEnd) - rowStart;
      runs.push({ byte, start: runStart, end: runEnd });
    }
    return runs;
  }, [selectedSignal, visibleSignals, messageSize]);

  // Pre-compute per-bit border info for the selected signal
  const selBorder = useMemo(() => {
    const map = new Map<number, { left: boolean; right: boolean; top: boolean; bottom: boolean }>();
    for (const run of selectedRuns) {
      for (let bit = run.start; bit <= run.end; bit++) {
        map.set(run.byte * 8 + bit, {
          left: bit === run.start,
          right: bit === run.end,
          top: true,
          bottom: true,
        });
      }
    }
    return map;
  }, [selectedRuns]);

  // ============== BLOCKS VIEW ==============
  if (viewMode === 'blocks') {
    // Build per-byte-row signal blocks
    // For each byte, group contiguous bits owned by the same signal into blocks
    const rows: React.ReactNode[] = [];
    for (let byte = 0; byte < messageSize; byte++) {
      const cells: React.ReactNode[] = [];
      let bit = 0;
      while (bit < 8) {
        const globalBit = byte * 8 + bit;
        const owner = bitOwners.get(globalBit);
        if (!owner) {
          cells.push(
            <Box key={bit} sx={{ width: 28, height: 28, display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: 'rgba(0,0,0,0.04)', borderRadius: 0.5 }}>
              <Typography variant="caption" sx={{ fontSize: '0.6rem', fontFamily: 'monospace', color: 'text.disabled' }}>{globalBit}</Typography>
            </Box>
          );
          bit++;
          continue;
        }
        // Count contiguous bits owned by same signal in this row
        let endBit = bit;
        while (endBit + 1 < 8 && bitOwners.get(byte * 8 + endBit + 1)?.name === owner.name) {
          endBit++;
        }
        const spanCount = endBit - bit + 1;
        const width = spanCount * 28 + (spanCount - 1) * 2; // 28px per cell + 2px gap between
        const sel = isSelected(owner.name);
        const style = getSignalStyle(owner);
        const showLabel = spanCount >= 2 || owner.size <= 2;

        cells.push(
          <Box
            key={bit}
            onClick={() => selectSignal(frameId, owner.name)}
            sx={{
              width, height: 28, display: 'flex', alignItems: 'center', justifyContent: 'center',
              bgcolor: style.bg,
              border: sel ? `2px solid ${style.border}` : `1px solid ${style.border}`,
              borderRadius: 0.5, cursor: 'pointer', px: 0.5,
              '&:hover': { borderColor: 'text.primary' },
            }}
            title={`${owner.name} [${owner.startBit}:${owner.startBit + owner.size - 1}] (${owner.size} bits)`}
          >
            <Typography variant="caption" sx={{ fontSize: '0.6rem', fontFamily: 'monospace', color: 'text.primary', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
              {showLabel ? owner.name : owner.name[0]}
            </Typography>
          </Box>
        );
        bit = endBit + 1;
      }
      rows.push(
        <Stack key={byte} direction="row" spacing={0.25} sx={{ alignItems: 'center' }}>
          <Typography variant="caption" sx={{ width: 24, fontSize: '0.6rem', color: 'text.disabled', fontFamily: 'monospace' }}>B{byte}</Typography>
          {cells}
        </Stack>
      );
    }

    return (
      <Box sx={{ mt: 1 }}>
        <Stack direction="row" spacing={1} sx={{ mb: 0.5, alignItems: 'center' }}>
          <Typography variant="subtitle2">Signal Layout</Typography>
          <ToggleButtonGroup size="small" exclusive value={viewMode} onChange={(_, v) => v && setViewMode(v)}>
            <ToggleButton value="bits">Bits</ToggleButton>
            <ToggleButton value="blocks">Blocks</ToggleButton>
          </ToggleButtonGroup>
        </Stack>
        {hasMultiplex && (
          <Box sx={{ mb: 1, display: 'flex', gap: 0.5, alignItems: 'center' }}>
            <Typography variant="caption" sx={{ color: 'text.secondary', mr: 0.5 }}>Mux:</Typography>
            <ToggleButtonGroup size="small" exclusive value={activeMuxValue === null ? 'all' : String(activeMuxValue)} onChange={(_, v) => setMuxFilter(v === 'all' ? null : Number(v))}>
              <ToggleButton value="all">All</ToggleButton>
              {muxSwitchValues.map(sv => (
                <ToggleButton key={sv} value={String(sv)}>m{sv}</ToggleButton>
              ))}
            </ToggleButtonGroup>
          </Box>
        )}
        <Stack spacing={0.25}>{rows}</Stack>
        <Stack direction="row" spacing={1.5} sx={{ mt: 0.5, flexWrap: 'wrap' }}>
          <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}><Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(74,158,255,0.15)', border: '1px solid rgba(74,158,255,0.3)' }} /><Typography variant="caption">Signal</Typography></Stack>
          {muxSwitchValuesMap.size > 0 && ([...muxSwitchValuesMap.values()].some(v => v === null) && (
            <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}><Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(168,85,247,0.3)', border: '1px solid #a855f7' }} /><Typography variant="caption">Switch</Typography></Stack>
          ))}
          {(() => {
            const usedValues = [...new Set([...muxSwitchValuesMap.values()].filter((v): v is number => v !== null))].sort((a, b) => a - b);
            return usedValues.map(sv => (
              <Stack key={sv} direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
                <Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: muxColors[sv % muxColors.length].bg, border: `1px solid ${muxColors[sv % muxColors.length].border}` }} />
                <Typography variant="caption">m{sv}</Typography>
              </Stack>
            ));
          })()}
          <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}><Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(233,69,96,0.18)', border: '2px solid #e94560' }} /><Typography variant="caption">Selected</Typography></Stack>
        </Stack>
      </Box>
    );
  }

  // ============== BITS VIEW ==============
  const rows: React.ReactNode[] = [];
  for (let byte = 0; byte < messageSize; byte++) {
    const cells: React.ReactNode[] = [];
    for (let bit = 0; bit < 8; bit++) {
      const bitIndex = byte * 8 + bit;
      const owner = bitOwners.get(bitIndex);
      const isStartBit = owner ? owner.startBit === bitIndex : false;
      const ownerName = owner?.name;
      const sel = ownerName ? isSelected(ownerName) : false;
      const muxValue = ownerName ? muxSwitchValuesMap.get(ownerName) : undefined;
      const isMuxSwitch = muxValue !== undefined && muxValue === null;
      const muxColorIdx = muxValue !== null && muxValue !== undefined ? muxValue % muxColors.length : -1;

      let bg = 'rgba(0,0,0,0.04)';
      let color = 'text.disabled';
      if (owner) {
        if (sel) { bg = 'rgba(233,69,96,0.18)'; }
        else if (isMuxSwitch) { bg = 'rgba(168,85,247,0.3)'; }
        else if (muxColorIdx >= 0) { bg = muxColors[muxColorIdx].bg; }
        else if (isStartBit) { bg = 'rgba(255,159,67,0.25)'; }
        else { bg = 'rgba(74,158,255,0.15)'; }
        color = 'text.primary';
      }

      // Per-cell border for selected signal outline
      const sb = selBorder.get(bitIndex);
      const borderLeft = sb?.left ? '2px solid #e94560' : '1px solid transparent';
      const borderRight = sb?.right ? '2px solid #e94560' : '1px solid transparent';
      const borderTop = sb?.top ? '2px solid #e94560' : sel ? '1px solid transparent' : '1px solid transparent';
      const borderBottom = sb?.bottom ? '2px solid #e94560' : sel ? '1px solid transparent' : '1px solid transparent';

      const normalBorder = owner
        ? (isMuxSwitch ? '1px solid #a855f7'
          : muxColorIdx >= 0 ? `1px solid ${muxColors[muxColorIdx].border}`
          : isStartBit ? '1px solid #ff9f43'
          : '1px solid rgba(74,158,255,0.3)')
        : '1px solid transparent';

      const finalBorderLeft = sb?.left ? borderLeft : normalBorder;
      const finalBorderRight = sb?.right ? borderRight : normalBorder;
      const finalBorderTop = sb ? borderTop : owner ? normalBorder : '1px solid transparent';
      const finalBorderBottom = sb ? borderBottom : owner ? normalBorder : '1px solid transparent';

      cells.push(
        <Box
          key={bitIndex}
          onClick={() => owner && selectSignal(frameId, owner.name)}
          sx={{
            width: 28, height: 28, display: 'flex', alignItems: 'center', justifyContent: 'center',
            borderRadius: 0.5, cursor: owner ? 'pointer' : 'default', bgcolor: bg,
            borderLeft: finalBorderLeft,
            borderRight: finalBorderRight,
            borderTop: finalBorderTop,
            borderBottom: finalBorderBottom,
            fontSize: '0.6rem', fontFamily: 'monospace', color, transition: 'background 0.15s',
            '&:hover': { borderColor: 'text.primary !important' },
            ml: bit === 0 ? 0 : '2px',
          }}
          title={owner ? `${owner.name} (bit ${bitIndex})` : `bit ${bitIndex}`}
        >
          {bitIndex}
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
    <Box sx={{ mt: 1 }}>
      <Stack direction="row" spacing={1} sx={{ mb: 0.5, alignItems: 'center' }}>
        <Typography variant="subtitle2">Bit Layout</Typography>
        <ToggleButtonGroup size="small" exclusive value={viewMode} onChange={(_, v) => v && setViewMode(v)}>
          <ToggleButton value="bits">Bits</ToggleButton>
          <ToggleButton value="blocks">Blocks</ToggleButton>
        </ToggleButtonGroup>
      </Stack>
      {hasMultiplex && (
        <Box sx={{ mb: 1, display: 'flex', gap: 0.5, alignItems: 'center' }}>
          <Typography variant="caption" sx={{ color: 'text.secondary', mr: 0.5 }}>Mux:</Typography>
          <ToggleButtonGroup size="small" exclusive value={activeMuxValue === null ? 'all' : String(activeMuxValue)} onChange={(_, v) => setMuxFilter(v === 'all' ? null : Number(v))}>
            <ToggleButton value="all">All</ToggleButton>
            {muxSwitchValues.map(sv => (
              <ToggleButton key={sv} value={String(sv)}>m{sv}</ToggleButton>
            ))}
          </ToggleButtonGroup>
        </Box>
      )}
      {rows}
      <Stack direction="row" spacing={1.5} sx={{ mt: 0.5, flexWrap: 'wrap' }}>
        <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}><Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(255,159,67,0.25)', border: '1px solid #ff9f43' }} /><Typography variant="caption">Start</Typography></Stack>
        <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}><Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(74,158,255,0.15)', border: '1px solid rgba(74,158,255,0.3)' }} /><Typography variant="caption">Signal</Typography></Stack>
        {muxSwitchValuesMap.size > 0 && ([...muxSwitchValuesMap.values()].some(v => v === null) && (
          <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}><Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(168,85,247,0.3)', border: '1px solid #a855f7' }} /><Typography variant="caption">Switch</Typography></Stack>
        ))}
        {(() => {
          const usedValues = [...new Set([...muxSwitchValuesMap.values()].filter((v): v is number => v !== null))].sort((a, b) => a - b);
          return usedValues.map(sv => (
            <Stack key={sv} direction="row" spacing={0.5} sx={{ alignItems: 'center' }}>
              <Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: muxColors[sv % muxColors.length].bg, border: `1px solid ${muxColors[sv % muxColors.length].border}` }} />
              <Typography variant="caption">m{sv}</Typography>
            </Stack>
          ));
        })()}
        <Stack direction="row" spacing={0.5} sx={{ alignItems: 'center' }}><Box sx={{ width: 12, height: 12, borderRadius: 0.25, bgcolor: 'rgba(233,69,96,0.18)', border: '2px solid #e94560' }} /><Typography variant="caption">Selected</Typography></Stack>
      </Stack>
    </Box>
  );
}