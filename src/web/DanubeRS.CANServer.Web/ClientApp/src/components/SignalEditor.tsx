import { useState, useEffect } from 'react';
import { Box, Typography, TextField, Button, Stack, ToggleButton, ToggleButtonGroup, Snackbar, Alert } from '@mui/material';
import { Save } from '@mui/icons-material';
import { useDbcStore } from '../store';
import { BitGrid } from './BitGrid';
import { NumberFieldMUI } from './NumberFieldMUI';

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

  const maxBit = message.size * 8;

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <Typography variant="subtitle1" sx={{ mb: 1, fontWeight: 600 }}>
        {signal.name}
        <Typography component="span" variant="caption" sx={{ ml: 1, color: 'text.secondary' }}>
          in {message.name} (0x{message.frameId.toString(16).toUpperCase()})
        </Typography>
      </Typography>

      <Box sx={{ display: 'flex', gap: 2, flex: 1, minHeight: 0 }}>
        <Box sx={{ flex: '0 0 auto' }}>
          {message && <BitGrid frameId={selectedSignal.frameId} messageSize={message.size} />}
        </Box>
        <Stack spacing={1.5} sx={{ flex: 1, minWidth: 220, overflow: 'auto' }}>
          <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 1.5 }}>
            <NumberFieldMUI label="Start Bit" value={values.startBit ?? signal.startBit} onChange={v => handleFieldChange('startBit', v)} min={0} max={maxBit - (values.size ?? signal.size)} step={1} />
            <NumberFieldMUI label="Size (bits)" value={values.size ?? signal.size} onChange={v => handleFieldChange('size', v)} min={1} max={maxBit - (values.startBit ?? signal.startBit)} step={1} />
            <NumberFieldMUI label="Factor" value={values.factor ?? signal.factor} onChange={v => handleFieldChange('factor', v)} step={0.01} />
            <NumberFieldMUI label="Offset" value={values.offset ?? signal.offset} onChange={v => handleFieldChange('offset', v)} step={0.01} />
            <NumberFieldMUI label="Min" value={values.min ?? signal.min} onChange={v => handleFieldChange('min', v)} step={0.01} />
            <NumberFieldMUI label="Max" value={values.max ?? signal.max} onChange={v => handleFieldChange('max', v)} step={0.01} />
          </Box>
          <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 1.5 }}>
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
          <Box>
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
                <NumberFieldMUI label="Switch Value" value={values.multiplex.switchValue ?? 0} onChange={v => handleFieldChange('multiplex', { ...values.multiplex, switchValue: v })} min={0} step={1} sx={{ width: 140 }} />
              )}
            </Box>
          </Box>
          <Stack direction="row" spacing={1}>
            <Button variant="contained" startIcon={<Save />} onClick={handleSave} disabled={saving} size="small">
              {saving ? 'Saving...' : 'Save'}
            </Button>
          </Stack>
        </Stack>
      </Box>
      <Snackbar open={!!feedback} autoHideDuration={4000} onClose={() => setFeedback(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
        <Alert severity={feedback?.type} onClose={() => setFeedback(null)} variant="filled">{feedback?.message}</Alert>
      </Snackbar>
    </Box>
  );
}