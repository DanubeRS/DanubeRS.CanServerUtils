import { useState, useRef } from 'react';
import { Box, TextField, Typography, IconButton, Collapse, Button, Dialog, DialogTitle, DialogContent, DialogActions, Stack, ToggleButton, ToggleButtonGroup, Alert } from '@mui/material';
import { ExpandMore, ExpandLess, UploadFile, Download, Delete, Add } from '@mui/icons-material';
import { useDbcStore } from '../store';
import { NumberFieldMUI } from './NumberFieldMUI';

export function SignalList() {
  const { loaded, messages, selectedSignal, selectSignal, loadDbc, removeMessage, exportDbc, loading, addMessage } = useDbcStore();
  const fileRef = useRef<HTMLInputElement>(null);
  const [filter, setFilter] = useState('');
  const [expanded, setExpanded] = useState<Set<number>>(new Set());
  const [addSignalFor, setAddSignalFor] = useState<{ frameId: number; messageName: string } | null>(null);
  const [showAddMsg, setShowAddMsg] = useState(false);
  const [addMsgError, setAddMsgError] = useState<string | null>(null);
  const [newMsgFrameId, setNewMsgFrameId] = useState('');
  const [newMsgName, setNewMsgName] = useState('');
  const [newMsgSize, setNewMsgSize] = useState(8);

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) await loadDbc(file);
    e.target.value = '';
  };

  const handleExport = async () => {
    const blob = await exportDbc();
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url; a.download = 'export.dbc'; a.click();
    URL.revokeObjectURL(url);
  };

  const toggleExpand = (id: number) => {
    setExpanded(prev => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id); else next.add(id);
      return next;
    });
  };

  const filtered = filter.toLowerCase();
  const filteredMessages = messages.filter(msg => {
    if (!filtered) return true;
    return msg.name.toLowerCase().includes(filtered)
      || msg.frameId.toString(16).toLowerCase().includes(filtered)
      || msg.signals.some(s => s.name.toLowerCase().includes(filtered));
  });

  if (!loaded) {
    return (
      <Box sx={{ p: 2, textAlign: 'center' }}>
        <Typography variant="body2" sx={{ mb: 2 }}>Load a DBC file to begin</Typography>
        <input ref={fileRef} type="file" accept=".dbc" onChange={handleFileUpload} hidden />
        <Button variant="contained" startIcon={<UploadFile />} onClick={() => fileRef.current?.click()} disabled={loading} size="small">
          {loading ? 'Loading...' : 'Load DBC'}
        </Button>
      </Box>
    );
  }

  const renderSignal = (msg: typeof messages[0], sig: typeof messages[0]['signals'][0], prefix: string, indentLevel: number) => (
    <Box
      key={sig.name}
      onClick={() => selectSignal(msg.frameId, sig.name)}
      sx={{
        pl: indentLevel, pr: 1, py: 0.25, cursor: 'pointer', fontSize: '0.8rem',
        bgcolor: selectedSignal?.frameId === msg.frameId && selectedSignal?.signalName === sig.name ? 'action.selected' : 'transparent',
        '&:hover': { bgcolor: 'action.hover' },
        borderLeft: selectedSignal?.frameId === msg.frameId && selectedSignal?.signalName === sig.name ? 2 : 0,
        borderColor: 'primary.main',
      }}
    >
      <Typography variant="body2" sx={{ fontSize: '0.8rem' }}>
        {prefix}{sig.name}
        <Typography component="span" variant="caption" sx={{ ml: 1, color: 'text.secondary', fontFamily: 'monospace' }}>
          {sig.startBit}|{sig.size}@{sig.order === 'Little' ? '1' : '0'}{sig.valueType === 'Unsigned' ? '+' : '-'}
        </Typography>
      </Typography>
    </Box>
  );

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ p: 1, borderBottom: 1, borderColor: 'divider', display: 'flex', gap: 0.5, alignItems: 'center' }}>
        <input ref={fileRef} type="file" accept=".dbc" onChange={handleFileUpload} hidden />
        <IconButton size="small" onClick={() => fileRef.current?.click()} disabled={loading} title="Load DBC"><UploadFile fontSize="small" /></IconButton>
        <IconButton size="small" onClick={handleExport} title="Export DBC"><Download fontSize="small" /></IconButton>
        <IconButton size="small" onClick={() => setShowAddMsg(true)} title="Add Message"><Add fontSize="small" /></IconButton>
        <Box sx={{ flexGrow: 1 }} />
      </Box>
      <Box sx={{ px: 1, py: 0.5 }}>
        <TextField size="small" placeholder="Filter messages/signals..." value={filter} onChange={e => setFilter(e.target.value)} fullWidth variant="outlined" sx={{ '& .MuiInputBase-input': { fontSize: '0.8rem', py: 0.5 } }} />
      </Box>
      <Box sx={{ flexGrow: 1, overflow: 'auto' }}>
        {filteredMessages.map(msg => {
          const switchSignal = msg.signals.find(s => s.multiplex?.isSwitch);
          const nonMultiplexed = msg.signals.filter(s => s.multiplex === null);
          const multiplexed = msg.signals.filter(s => s.multiplex && !s.multiplex.isSwitch);
          const switchValues = [...new Set(multiplexed.map(s => s.multiplex!.switchValue!))].sort((a, b) => a - b);

          return (
            <Box key={msg.frameId}>
              <Box
                sx={{ display: 'flex', alignItems: 'center', px: 1, py: 0.5, cursor: 'pointer', '&:hover': { bgcolor: 'action.hover' } }}
                onClick={() => toggleExpand(msg.frameId)}
              >
                <IconButton size="small" sx={{ mr: 0.5, p: 0 }}>
                  {expanded.has(msg.frameId) ? <ExpandLess fontSize="small" /> : <ExpandMore fontSize="small" />}
                </IconButton>
                <Typography variant="caption" sx={{ fontFamily: 'monospace', color: 'error.main', minWidth: 48 }}>
                  0x{msg.frameId.toString(16).toUpperCase()}
                </Typography>
                <Typography variant="body2" sx={{ flex: 1, fontWeight: 500, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  {msg.name}
                </Typography>
                <Typography variant="caption" sx={{ color: 'text.secondary', ml: 0.5 }}>{msg.size}B</Typography>
                <IconButton size="small" onClick={e => { e.stopPropagation(); setAddSignalFor({ frameId: msg.frameId, messageName: msg.name }); }} title="Add signal"><Add fontSize="small" /></IconButton>
                <IconButton size="small" onClick={e => { e.stopPropagation(); removeMessage(msg.frameId); }} title="Remove message"><Delete fontSize="small" /></IconButton>
              </Box>
              <Collapse in={expanded.has(msg.frameId)}>
                {switchSignal && renderSignal(msg, switchSignal, '⬦ ', 3)}
                {switchValues.map(sv => (
                  <Box key={sv} sx={{ pl: 2 }}>
                    <Typography variant="caption" sx={{ color: 'text.secondary', fontWeight: 600, py: 0.25, display: 'block' }}>
                      ◇ m{sv}
                    </Typography>
                    {multiplexed
                      .filter(s => s.multiplex!.switchValue === sv)
                      .filter(sig => !filtered || sig.name.toLowerCase().includes(filtered))
                      .map(sig => renderSignal(msg, sig, '', 4))}
                  </Box>
                ))}
                {nonMultiplexed
                  .filter(sig => !filtered || sig.name.toLowerCase().includes(filtered))
                  .map(sig => renderSignal(msg, sig, '', 4))}
              </Collapse>
            </Box>
          );
        })}
      </Box>

      <AddSignalDialog
        open={!!addSignalFor}
        frameId={addSignalFor?.frameId ?? 0}
        messageName={addSignalFor?.messageName ?? ''}
        onClose={() => setAddSignalFor(null)}
      />

      <Dialog open={showAddMsg} onClose={() => setShowAddMsg(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Add Message</DialogTitle>
        <DialogContent>
          <Stack spacing={1.5} sx={{ mt: 1 }}>
            <TextField label="Frame ID (hex e.g. 100)" size="small" value={newMsgFrameId} onChange={e => setNewMsgFrameId(e.target.value)} autoFocus />
            <TextField label="Name" size="small" value={newMsgName} onChange={e => setNewMsgName(e.target.value)} />
            <TextField label="Size (bytes)" type="number" size="small" value={newMsgSize} onChange={e => setNewMsgSize(Number(e.target.value))} />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowAddMsg(false)}>Cancel</Button>
          <Button onClick={async () => {
            const frameId = parseInt(newMsgFrameId, 16);
            if (isNaN(frameId) || !newMsgName.trim()) return;
            try {
              await addMessage({ frameId, name: newMsgName.trim(), size: newMsgSize });
              setShowAddMsg(false);
              setNewMsgFrameId('');
              setNewMsgName('');
              setNewMsgSize(8);
            } catch (e: any) {
              setAddMsgError(e?.message || 'Failed to add message');
            }
          }} variant="contained" disabled={!newMsgFrameId.trim() || !newMsgName.trim()}>Add</Button>
        </DialogActions>
        {addMsgError && <Box sx={{ px: 3, pb: 2 }}><Alert severity="error" onClose={() => setAddMsgError(null)}>{addMsgError}</Alert></Box>}
      </Dialog>
    </Box>
  );
}

function AddSignalDialog({ open, frameId, messageName, onClose }: { open: boolean; frameId: number; messageName: string; onClose: () => void }) {
  const addSignal = useDbcStore(s => s.addSignal);
  const [name, setName] = useState('');
  const [startBit, setStartBit] = useState(0);
  const [size, setSize] = useState(1);
  const [order, setOrder] = useState<string>('Little');
  const [valueType, setValueType] = useState<string>('Unsigned');
  const [factor, setFactor] = useState(1);
  const [offset, setOffset] = useState(0);
  const [min, setMin] = useState(0);
  const [max, setMax] = useState(1);
  const [unit, setUnit] = useState('');
  const [receiver, setReceiver] = useState('Vector');
  const [multiplexMode, setMultiplexMode] = useState<'none' | 'switch' | 'multiplexed'>('none');
  const [switchValue, setSwitchValue] = useState(0);
  const [adding, setAdding] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleAdd = async () => {
    if (!name.trim()) return;
    setAdding(true);
    setError(null);
    const multiplex = multiplexMode === 'none' ? null
      : multiplexMode === 'switch' ? { isSwitch: true, switchValue: null }
      : { isSwitch: false, switchValue };
    try {
      await addSignal(frameId, {
        name: name.trim(), startBit, size, order, valueType, factor, offset, min, max, unit, receiver,
        multiplex: multiplex as any,
      });
      resetForm();
      onClose();
    } catch (e: any) {
      setError(e?.message || 'Failed to add signal');
    } finally {
      setAdding(false);
    }
  };

  const resetForm = () => {
    setName(''); setStartBit(0); setSize(1); setFactor(1); setOffset(0);
    setMin(0); setMax(1); setUnit(''); setReceiver('Vector');
    setOrder('Little'); setValueType('Unsigned'); setMultiplexMode('none'); setSwitchValue(0);
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Signal to {messageName} (0x{frameId.toString(16).toUpperCase()})</DialogTitle>
      <DialogContent>
        <Stack spacing={1.5} sx={{ mt: 1 }}>
          <TextField label="Name" size="small" value={name} onChange={e => setName(e.target.value)} autoFocus />
          <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 1.5 }}>
            <NumberFieldMUI label="Start Bit" value={startBit} onChange={setStartBit} min={0} step={1} />
          <NumberFieldMUI label="Size (bits)" value={size} onChange={setSize} min={1} step={1} />
          <NumberFieldMUI label="Factor" value={factor} onChange={setFactor} step={0.01} />
          <NumberFieldMUI label="Offset" value={offset} onChange={setOffset} step={0.01} />
          <NumberFieldMUI label="Min" value={min} onChange={setMin} step={0.01} />
          <NumberFieldMUI label="Max" value={max} onChange={setMax} step={0.01} />
          </Box>
          <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 1.5 }}>
            <ToggleButtonGroup size="small" exclusive value={order} onChange={(_, val) => val && setOrder(val)}>
              <ToggleButton value="Little">Little</ToggleButton>
              <ToggleButton value="Big">Big</ToggleButton>
            </ToggleButtonGroup>
            <ToggleButtonGroup size="small" exclusive value={valueType} onChange={(_, val) => val && setValueType(val)}>
              <ToggleButton value="Unsigned">Unsigned</ToggleButton>
              <ToggleButton value="Signed">Signed</ToggleButton>
            </ToggleButtonGroup>
          </Box>
          <TextField label="Unit" size="small" value={unit} onChange={e => setUnit(e.target.value)} />
          <TextField label="Receiver" size="small" value={receiver} onChange={e => setReceiver(e.target.value)} />
          <Box>
            <Typography variant="caption" sx={{ color: 'text.secondary', mb: 0.5 }}>Multiplex</Typography>
            <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
              <ToggleButtonGroup size="small" exclusive value={multiplexMode} onChange={(_, val) => val && setMultiplexMode(val)}>
                <ToggleButton value="none">None</ToggleButton>
                <ToggleButton value="switch">Switch</ToggleButton>
                <ToggleButton value="multiplexed">Mux</ToggleButton>
              </ToggleButtonGroup>
              {multiplexMode === 'multiplexed' && (
                <NumberFieldMUI label="Switch Value" value={switchValue} onChange={setSwitchValue} min={0} step={1} />
              )}
            </Box>
          </Box>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button onClick={handleAdd} variant="contained" disabled={!name.trim() || adding}>{adding ? 'Adding...' : 'Add Signal'}</Button>
      </DialogActions>
      {error && <Box sx={{ px: 3, pb: 2 }}><Alert severity="error" onClose={() => setError(null)}>{error}</Alert></Box>}
    </Dialog>
  );
}