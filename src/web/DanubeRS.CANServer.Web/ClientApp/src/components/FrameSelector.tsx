import { useMemo, useState } from 'react';
import { Autocomplete, TextField } from '@mui/material';
import { useDbcStore } from '../store';

function parseFrameId(input: string): number | null {
  const trimmed = input.trim();
  if (!trimmed) return null;
  if (trimmed.startsWith('0x') || trimmed.startsWith('0X'))
    return parseInt(trimmed.slice(2), 16);
  if (/^[0-9a-fA-F]{3,}$/.test(trimmed) && trimmed.length > 4)
    return parseInt(trimmed, 16);
  const n = Number(trimmed);
  return isNaN(n) ? null : n;
}

function formatFrameId(id: number): string {
  const msg = useDbcStore.getState().messages.find(m => m.frameId === id);
  const hex = `0x${id.toString(16).toUpperCase()}`;
  return msg ? `${msg.name} (${hex})` : hex;
}

export function FrameSelector() {
  const logLoaded = useDbcStore(s => s.logLoaded);
  const pandasConnected = useDbcStore(s => s.pandasConnected);
  const liveFrameIds = useDbcStore(s => s.liveFrameIds);

  const selectedFrameId = useDbcStore(s => s.selectedFrameId);
  const selectFrameId = useDbcStore(s => s.selectFrameId);
  const loadAnalysisFrames = useDbcStore(s => s.loadAnalysisFrames);
  const messages = useDbcStore(s => s.messages);
  const frames = useDbcStore(s => s.frames);

  const [inputValue, setInputValue] = useState('');

  const frameIds = useMemo(() => {
    if (logLoaded) {
      const ids = frames.map(f => f.frameId).filter((v, i, a) => a.indexOf(v) === i);
      return ids.sort((a, b) => {
        const mA = messages.find(m => m.frameId === a);
        const mB = messages.find(m => m.frameId === b);
        const nameA = mA?.name ?? '';
        const nameB = mB?.name ?? '';
        if (nameA && nameB) return nameA.localeCompare(nameB);
        if (nameA && !nameB) return -1;
        if (!nameA && nameB) return 1;
        return a - b;
      });
    }
    if (pandasConnected && liveFrameIds.length > 0) {
      const knownIds = new Set(messages.map(m => m.frameId));
      const sorted = [...liveFrameIds].sort((a, b) => a - b);
      const named = [...messages].sort((a, b) => a.name.localeCompare(b.name)).map(m => m.frameId);
      const unnamed = sorted.filter(id => !knownIds.has(id));
      return [...named, ...unnamed];
    }
    return [...messages].sort((a, b) => a.name.localeCompare(b.name)).map(m => m.frameId);
  }, [logLoaded, pandasConnected, liveFrameIds, frames, messages]);

  const handleChange = async (frameId: number | null) => {
    if (frameId === null) return;
    selectFrameId(frameId);
    if (logLoaded) {
      await loadAnalysisFrames(frameId);
    }
  };

  const options = frameIds.map(id => ({
    id,
    label: formatFrameId(id),
  }));

  const currentOption = selectedFrameId !== null
    ? { id: selectedFrameId, label: formatFrameId(selectedFrameId) }
    : null;

  return (
    <Autocomplete
      fullWidth
      size="small"
      sx={{ mb: 2 }}
      options={options}
      getOptionLabel={(o) => typeof o === 'string' ? o : o.label}
      isOptionEqualToValue={(o, v) => typeof v !== 'string' && o.id === v.id}
      value={currentOption}
      inputValue={inputValue}
      onInputChange={(_, newInput) => setInputValue(newInput)}
      onChange={(_, value) => {
        if (value && typeof value !== 'string') {
          handleChange(value.id);
        } else if (typeof value === 'string') {
          const parsed = parseFrameId(value);
          if (parsed !== null) handleChange(parsed);
        }
      }}
      freeSolo={pandasConnected && !logLoaded}
      renderInput={(params) => <TextField {...params} label="Frame" placeholder="0x1A2 or 418" />}
    />
  );
}