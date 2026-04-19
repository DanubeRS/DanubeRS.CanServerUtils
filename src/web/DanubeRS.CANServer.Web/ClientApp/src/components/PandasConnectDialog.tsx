import { useState } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField, Stack, Alert } from '@mui/material';
import { useDbcStore } from '../store';

interface PandasConnectDialogProps {
  open: boolean;
  onClose: () => void;
}

export function PandasConnectDialog({ open, onClose }: PandasConnectDialogProps) {
  const connectPandas = useDbcStore(s => s.connectPandas);
  const disconnectPandas = useDbcStore(s => s.disconnectPandas);
  const pandasConnected = useDbcStore(s => s.pandasConnected);
  const [ip, setIp] = useState('192.168.4.1');
  const [port, setPort] = useState(1338);
  const [error, setError] = useState<string | null>(null);
  const [connecting, setConnecting] = useState(false);

  const handleConnect = async () => {
    setError(null);
    setConnecting(true);
    try {
      await connectPandas(ip, port);
      onClose();
    } catch (e: any) {
      setError(e.message);
    } finally {
      setConnecting(false);
    }
  };

  const handleDisconnect = async () => {
    await disconnectPandas();
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{pandasConnected ? 'Panda Connected' : 'Connect to Panda'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {error && <Alert severity="error" onClose={() => setError(null)}>{error}</Alert>}
          <TextField
            label="IP Address"
            size="small"
            value={ip}
            onChange={e => setIp(e.target.value)}
            disabled={pandasConnected || connecting}
            fullWidth
          />
          <TextField
            label="Port"
            type="number"
            size="small"
            value={port}
            onChange={e => setPort(Number(e.target.value))}
            disabled={pandasConnected || connecting}
            fullWidth
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        {pandasConnected ? (
          <Button onClick={handleDisconnect} color="error" variant="contained">Disconnect</Button>
        ) : (
          <Button onClick={handleConnect} variant="contained" disabled={connecting || !ip.trim()}>
            {connecting ? 'Connecting...' : 'Connect'}
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
}