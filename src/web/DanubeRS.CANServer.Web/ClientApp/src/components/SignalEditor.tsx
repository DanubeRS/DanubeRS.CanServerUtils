import { useState, useEffect } from 'react';
import { Box, Typography, TextField, Button, Stack, ToggleButton, ToggleButtonGroup, Snackbar, Alert } from '@mui/material';
import { Save } from '@mui/icons-material';
import { useDbcStore } from '../store';
import { BitGrid } from './BitGrid';

export function SignalEditor() {
  const { messages, selectedSignal, updateSignal } = useDbcStore();
  const [values, setValues] = useState<Record<string, any>>({});
  const [saving, setSaving] = useState(false);
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const signal = selectedSignal
    ? messages.find(m => m.frameId === selectedSignal.frameId)?.signals.find(s => s.name === selectedSignal.signalName)
    : null;

  const message = selectedSignal
    ? messages.find(m => m.frameId === selectedSignal.frameId)
    : null;

  useEffect(() => {
    if (signal) {
      setValues({
        startBit: signal.startBit,
        size: signal.size,
        factor: signal.factor,
        offset: signal.offset,
        min: signal.min,
        max: signal.max,
        unit: signal.unit,
        order: signal.order,
        valueType: signal.valueType,
        receiver: signal.receiver,
        multiplex: signal.multiplex
          ? { isSwitch: signal.multiplex.isSwitch, switchValue: signal.multiplex.switchValue }
          : null,
      });
    }
  }, [signal?.name, signal?.startBit]);

  if (!signal || !message || !selectedSignal) return null;

  const handleSave = async () => {
    setSaving(true);
    setFeedback(null);
    const update: Record<string, any> = { ...values };
    if (values.multiplex === null) {
      update.clearMultiplex = true;
    }
    try {
      await updateSignal(selectedSignal.frameId, signal.name, update);
      setFeedback({ type: 'success', message: 'Signal saved' });
    } catch (e: any) {
      setFeedback({ type: 'error', message: e?.message || 'Save failed' });
    } finally {
      setSaving(false);
    }
  };

  const handleFieldChange = (field: string, value: any) => {
    setValues(v => ({ ...v, [field]: value }));
  };

  return (
    <Box>
      <Typography variant="subtitle1" sx={{ mb: 1, fontWeight: 600 }}>
        {signal.name}
        <Typography component="span" variant="caption" sx={{ ml: 1, color: 'text.secondary' }}>
          in {message.name} (0x{message.frameId.toString(16).toUpperCase()})
        </Typography>
      </Typography>

      {message && <BitGrid frameId={selectedSignal.frameId} messageSize={message.size} />}

      <Stack spacing={1.5} sx={{ mt: 2 }}>
        <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 1.5 }}>
          <TextField label="Start Bit" type="number" size="small" value={values.startBit ?? signal.startBit} onChange={e => handleFieldChange('startBit', Number(e.target.value))} />
          <TextField label="Size" type="number" size="small" value={values.size ?? signal.size} onChange={e => handleFieldChange('size', Number(e.target.value))} />
          <TextField label="Factor" type="number" size="small" value={values.factor ?? signal.factor} onChange={e => handleFieldChange('factor', Number(e.target.value))} />
          <TextField label="Offset" type="number" size="small" value={values.offset ?? signal.offset} onChange={e => handleFieldChange('offset', Number(e.target.value))} />
          <TextField label="Min" type="number" size="small" value={values.min ?? signal.min} onChange={e => handleFieldChange('min', Number(e.target.value))} />
          <TextField label="Max" type="number" size="small" value={values.max ?? signal.max} onChange={e => handleFieldChange('max', Number(e.target.value))} />
          <ToggleButtonGroup
            size="small"
            exclusive
            value={values.order ?? signal.order}
            onChange={(_, val) => val && handleFieldChange('order', val)}
          >
            <ToggleButton value="Little">Little</ToggleButton>
            <ToggleButton value="Big">Big</ToggleButton>
          </ToggleButtonGroup>
          <ToggleButtonGroup
            size="small"
            exclusive
            value={values.valueType ?? signal.valueType}
            onChange={(_, val) => val && handleFieldChange('valueType', val)}
          >
            <ToggleButton value="Unsigned">Unsigned</ToggleButton>
            <ToggleButton value="Signed">Signed</ToggleButton>
          </ToggleButtonGroup>
        </Box>
        <TextField label="Unit" size="small" value={values.unit ?? signal.unit} onChange={e => handleFieldChange('unit', e.target.value)} />
        <TextField label="Receiver" size="small" value={values.receiver ?? signal.receiver} onChange={e => handleFieldChange('receiver', e.target.value)} />
        <Box sx={{ mt: 1 }}>
          <Typography variant="caption" sx={{ color: 'text.secondary', mb: 0.5 }}>Multiplex</Typography>
          <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
            <ToggleButtonGroup
              size="small"
              exclusive
              value={
                values.multiplex === null ? 'none'
                : values.multiplex?.isSwitch ? 'switch'
                : 'multiplexed'
              }
              onChange={(_, val) => {
                if (!val) return;
                if (val === 'none') handleFieldChange('multiplex', null);
                else if (val === 'switch') handleFieldChange('multiplex', { isSwitch: true, switchValue: null });
                else handleFieldChange('multiplex', { isSwitch: false, switchValue: 0 });
              }}
            >
              <ToggleButton value="none">None</ToggleButton>
              <ToggleButton value="switch">Switch</ToggleButton>
              <ToggleButton value="multiplexed">Mux</ToggleButton>
            </ToggleButtonGroup>
            {values.multiplex && !values.multiplex.isSwitch && (
              <TextField
                label="Switch Value"
                type="number"
                size="small"
                value={values.multiplex.switchValue ?? 0}
                onChange={e => handleFieldChange('multiplex', { ...values.multiplex, switchValue: Number(e.target.value) })}
                sx={{ width: 120 }}
              />
            )}
          </Box>
        </Box>
        <Stack direction="row" spacing={1} sx={{ mt: 1 }}>
          <Button variant="contained" startIcon={<Save />} onClick={handleSave} disabled={saving} size="small">
            {saving ? 'Saving...' : 'Save'}
          </Button>
        </Stack>
      </Stack>
      <Snackbar open={!!feedback} autoHideDuration={4000} onClose={() => setFeedback(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
        <Alert severity={feedback?.type} onClose={() => setFeedback(null)} variant="filled">{feedback?.message}</Alert>
      </Snackbar>
    </Box>
  );
}