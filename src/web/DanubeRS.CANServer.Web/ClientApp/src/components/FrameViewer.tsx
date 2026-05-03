import { useState, useRef, useMemo } from 'react';
import { Box, Typography, Table, TableBody, TableCell, TableHead, TableRow, Button, TextField, IconButton } from '@mui/material';
import { UploadFile } from '@mui/icons-material';
import { useDbcStore } from '../store';
import type { RawFrameInfo } from '../api';
import * as api from '../api';

function formatPayload(payload: number[]): string {
  return payload.map(b => b.toString(16).padStart(2, '0').toUpperCase()).join(' ');
}

export function FrameViewer() {
  const { logLoaded, pandasConnected, frames, selectedSignal, messages, loadLog, loading, liveFrameHistory, liveFrameIds, liveFrameHistoryVersion } = useDbcStore();
  const fileRef = useRef<HTMLInputElement>(null);
  const [frameDetail, setFrameDetail] = useState<api.FrameDetail | null>(null);
  const [previewResult, setPreviewResult] = useState<api.DecodedFrame | null>(null);
  const [filterId, setFilterId] = useState('');

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) await loadLog(file);
    e.target.value = '';
  };

  const handleFrameClick = async (index: number) => {
    const detail = await api.getFrame(index);
    setFrameDetail(detail);
    setPreviewResult(null);
  };

  const handlePreview = async () => {
    if (!frameDetail || !selectedSignal) return;
    const msg = messages.find(m => m.frameId === selectedSignal.frameId);
    if (!msg) return;
    const sig = msg.signals.find(s => s.name === selectedSignal.signalName);
    if (!sig) return;
    const overrides: Record<string, api.SignalOverride> = {};
    overrides[sig.name] = { startBit: sig.startBit, size: sig.size, factor: sig.factor, offset: sig.offset };
    const result = await api.previewDecode({ frameIndex: frameDetail.index, signalOverrides: overrides });
    setPreviewResult(result);
  };

  const liveFramesForId = useMemo(() => {
    if (!pandasConnected || !filterId) return [];
    const id = parseInt(filterId, 16);
    if (isNaN(id)) return [];
    return liveFrameHistory.get(id) ?? [];
  }, [pandasConnected, filterId, liveFrameHistoryVersion, liveFrameHistory]);

  const liveAllFrames = useMemo(() => {
    if (!pandasConnected || filterId) return [];
    const all: RawFrameInfo[] = [];
    for (const [, hist] of liveFrameHistory) {
      all.push(...hist);
    }
    all.sort((a, b) => a.frameTime - b.frameTime);
    return all;
  }, [pandasConnected, filterId, liveFrameHistoryVersion, liveFrameHistory]);

  const filteredFrames = filterId
    ? frames.filter(f => f.frameId === parseInt(filterId, 16))
    : frames;

  const displayFrames = pandasConnected
    ? (filterId ? liveFramesForId : liveAllFrames)
    : filteredFrames;

  if (!logLoaded && !pandasConnected) {
    return (
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
        <Typography variant="body1" sx={{ mb: 2 }}>Load a CAN log file or connect to Panda to view frames</Typography>
        <input ref={fileRef} type="file" accept=".bin,.log" onChange={handleFileUpload} hidden />
        <Button variant="contained" startIcon={<UploadFile />} onClick={() => fileRef.current?.click()} disabled={loading}>
          {loading ? 'Loading...' : 'Load Log File'}
        </Button>
      </Box>
    );
  }

  return (
    <Box sx={{ display: 'flex', height: '100%', overflow: 'hidden' }}>
      <Box sx={{ width: 340, minWidth: 340, borderRight: 1, borderColor: 'divider', display: 'flex', flexDirection: 'column' }}>
        <Box sx={{ p: 1, display: 'flex', gap: 1, alignItems: 'center' }}>
          {!pandasConnected && (
            <>
              <input ref={fileRef} type="file" accept=".bin,.log" onChange={handleFileUpload} hidden />
              <IconButton size="small" onClick={() => fileRef.current?.click()} disabled={loading} title="Reload Log"><UploadFile fontSize="small" /></IconButton>
            </>
          )}
          <TextField size="small" placeholder="Filter by ID (hex)" value={filterId} onChange={e => setFilterId(e.target.value)} fullWidth />
          {pandasConnected && (
            <Typography variant="caption" sx={{ color: 'text.secondary', whiteSpace: 'nowrap' }}>
              {liveFrameIds.length} IDs
            </Typography>
          )}
        </Box>
        <Box sx={{ flexGrow: 1, overflow: 'auto' }}>
          {pandasConnected ? (
            displayFrames.length === 0 ? (
              <Box sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="body2" color="text.secondary">
                  {filterId ? 'No frames for this ID yet' : 'Track a frame to see data'}
                </Typography>
              </Box>
            ) : (
              displayFrames.map((f, i) => (
                <Box key={i} sx={{ px: 1, py: 0.25, cursor: 'pointer', '&:hover': { bgcolor: 'action.hover' }, fontSize: '0.8rem' }}>
                  <Typography variant="caption" sx={{ fontFamily: 'monospace', color: 'text.secondary', mr: 1 }}>
                    {(f.frameTime / 1000).toFixed(3)}s
                  </Typography>
                  <Typography component="span" variant="caption" sx={{ fontFamily: 'monospace', color: 'error.main', mr: 1 }}>
                    0x{f.frameId.toString(16).toUpperCase()}
                  </Typography>
                  <Typography component="span" variant="caption" sx={{ color: 'text.secondary', fontFamily: 'monospace' }}>
                    {formatPayload(f.payload).substring(0, 23)}{f.payload.length > 8 ? '...' : ''}
                  </Typography>
                </Box>
              ))
            )
          ) : (
            filteredFrames.map(f => (
              <Box key={f.index} onClick={() => handleFrameClick(f.index)} sx={{ px: 1, py: 0.25, cursor: 'pointer', '&:hover': { bgcolor: 'action.hover' }, fontSize: '0.8rem' }}>
                <Typography variant="caption" sx={{ fontFamily: 'monospace', color: 'text.secondary', mr: 1 }}>
                  {(f.frameTime / 1000000).toFixed(3)}s
                </Typography>
                <Typography component="span" variant="caption" sx={{ fontFamily: 'monospace', color: 'error.main', mr: 1 }}>
                  0x{f.frameId.toString(16).toUpperCase()}
                </Typography>
                <Typography component="span" variant="caption" sx={{ color: 'primary.main' }}>
                  {f.messageName ?? '—'}
                </Typography>
              </Box>
            ))
          )}
        </Box>
      </Box>
      <Box sx={{ flex: 1, overflow: 'auto', p: 2 }}>
        {frameDetail ? (
          <>
            <Typography variant="subtitle1" sx={{ mb: 1 }}>
              Frame {frameDetail.index}: 0x{frameDetail.frameId.toString(16).toUpperCase()}
            </Typography>
            <Box sx={{ display: 'flex', gap: 0.5, mb: 2, flexWrap: 'wrap' }}>
              {Array.from(frameDetail.payload).map((b, i) => (
                <Typography key={i} variant="caption" sx={{ fontFamily: 'monospace', bgcolor: 'action.hover', px: 0.5, borderRadius: 0.5 }}>
                  {b.toString(16).padStart(2, '0').toUpperCase()}
                </Typography>
              ))}
            </Box>
            {frameDetail.decoded && (
              <>
                <Typography variant="subtitle2">Decoded Signals</Typography>
                <Table size="small">
                  <TableHead><TableRow><TableCell>Signal</TableCell><TableCell align="right">Value</TableCell><TableCell>Unit</TableCell></TableRow></TableHead>
                  <TableBody>
                    {frameDetail.decoded.signals.map(s => (
                      <TableRow key={s.name}><TableCell>{s.name}</TableCell><TableCell align="right">{s.value.toFixed(3)}</TableCell><TableCell>{s.unit}</TableCell></TableRow>
                    ))}
                  </TableBody>
                </Table>
              </>
            )}
            {selectedSignal && <Button size="small" variant="outlined" onClick={handlePreview} sx={{ mt: 1 }}>Preview Decode</Button>}
            {previewResult && (
              <Box sx={{ mt: 1, p: 1, border: 1, borderColor: 'primary.main', borderRadius: 1 }}>
                <Typography variant="subtitle2" sx={{ color: 'primary.main' }}>Preview Result</Typography>
                <Table size="small">
                  <TableBody>
                    {previewResult.signals.map(s => (
                      <TableRow key={s.name}><TableCell>{s.name}</TableCell><TableCell align="right">{s.value.toFixed(3)}</TableCell><TableCell>{s.unit}</TableCell></TableRow>
                    ))}
                  </TableBody>
                </Table>
              </Box>
            )}
          </>
        ) : (
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%', opacity: 0.4 }}>
            {pandasConnected ? 'Frames will appear as they arrive' : 'Select a frame to view details'}
          </Box>
        )}
      </Box>
    </Box>
  );
}