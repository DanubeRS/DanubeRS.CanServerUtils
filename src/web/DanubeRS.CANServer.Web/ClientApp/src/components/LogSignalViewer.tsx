import { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import { Box, Typography, IconButton, Alert, CircularProgress, Chip } from '@mui/material';
import { UploadFile } from '@mui/icons-material';
import { Plot } from './Plot';
import * as api from '../api';
import { useDbcStore } from '../store';

export function LogSignalViewer() {
  const selectedSignal = useDbcStore(s => s.selectedSignal);
  const logLoaded = useDbcStore(s => s.logLoaded);
  const pandasConnected = useDbcStore(s => s.pandasConnected);
  const liveSignalData = useDbcStore(s => s.liveSignalData);
  const liveVersion = useDbcStore(s => s.liveVersion);
  const loading = useDbcStore(s => s.loading);
  const loadLog = useDbcStore(s => s.loadLog);
  const signalVersion = useDbcStore(s => s.signalVersion);
  const messages = useDbcStore(s => s.messages);
  const fileRef = useRef<HTMLInputElement>(null);
  const [timeSeries, setTimeSeries] = useState<api.TimeSeriesPoint[]>([]);
  const [chartLoading, setChartLoading] = useState(false);
  const [chartError, setChartError] = useState<string | null>(null);

  const fetchTimeSeries = useCallback(async () => {
    if (!selectedSignal || !logLoaded) {
      setTimeSeries([]);
      return;
    }
    setChartLoading(true);
    setChartError(null);
    try {
      const data = await api.decodeSignalTimeSeries(selectedSignal.frameId, selectedSignal.signalName);
      setTimeSeries(data);
    } catch (e: any) {
      console.error('Time series fetch failed:', e);
      setChartError(e.message);
      setTimeSeries([]);
    } finally {
      setChartLoading(false);
    }
  }, [selectedSignal?.frameId, selectedSignal?.signalName, logLoaded, signalVersion]);

  useEffect(() => {
    fetchTimeSeries();
  }, [fetchTimeSeries]);

  const liveData = useMemo(() => {
    if (!selectedSignal || !pandasConnected) return [];
    const key = `${selectedSignal.frameId}:${selectedSignal.signalName}`;
    return liveSignalData.get(key) ?? [];
  }, [selectedSignal?.frameId, selectedSignal?.signalName, pandasConnected, liveVersion]);

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) await loadLog(file);
    e.target.value = '';
  };

  if (!logLoaded && !pandasConnected) {
    return (
      <Box sx={{ p: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
        <Typography variant="caption" sx={{ color: 'text.secondary' }}>
          Load a log or connect to Panda to see decoded signal values
        </Typography>
        <input ref={fileRef} type="file" accept=".bin,.log" onChange={handleFileUpload} hidden />
        <IconButton size="small" onClick={() => fileRef.current?.click()} disabled={loading} title="Load Log">
          <UploadFile fontSize="small" />
        </IconButton>
      </Box>
    );
  }

  if (!selectedSignal) {
    return (
      <Box sx={{ p: 1 }}>
        <Typography variant="caption" sx={{ color: 'text.secondary' }}>
          Select a signal to view its time-series graph
        </Typography>
      </Box>
    );
  }

  const signalName = selectedSignal.signalName;
  const unit = messages
    .find(m => m.frameId === selectedSignal.frameId)
    ?.signals.find(s => s.name === signalName)?.unit ?? '';

  const displayData = pandasConnected ? liveData : timeSeries;
  const t0 = displayData.length > 0 ? displayData[0].time : 0;
  const xData = displayData.map(p => (p.time - t0) / 1000);
  const yData = displayData.map(p => p.value);

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 0.5 }}>
        <Typography variant="subtitle2">
          {signalName}{unit ? ` (${unit})` : ''} — {displayData.length} points
        </Typography>
        {pandasConnected && (
          <Chip label="LIVE" size="small" color="success" sx={{ height: 20, fontSize: '0.65rem' }} />
        )}
      </Box>

      {chartLoading && !pandasConnected && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 2 }}>
          <CircularProgress size={24} />
        </Box>
      )}

      {chartError && !pandasConnected && (
        <Alert severity="error" onClose={() => setChartError(null)} sx={{ mb: 1 }}>
          {chartError}
        </Alert>
      )}

      {displayData.length > 0 && (
        <Plot
          data={[{
            x: xData,
            y: yData,
            type: 'scattergl',
            mode: 'lines',
            line: { color: '#90caf9', width: 1.5 },
            name: signalName,
          }]}
          layout={{
            margin: { t: 10, r: 20, b: 35, l: 50 },
            paper_bgcolor: 'transparent',
            plot_bgcolor: 'transparent',
            xaxis: {
              title: { text: 'Time (s)', font: { size: 10, color: '#aaa' } },
              tickfont: { size: 9, color: '#aaa' },
              gridcolor: '#444',
              zerolinecolor: '#555',
            },
            yaxis: {
              title: { text: unit || 'Value', font: { size: 10, color: '#aaa' } },
              tickfont: { size: 9, color: '#aaa' },
              gridcolor: '#444',
              zerolinecolor: '#555',
            },
            font: { color: '#aaa' },
            dragmode: 'zoom',
            showlegend: false,
          }}
          config={{
            responsive: true,
            displayModeBar: true,
            modeBarButtonsToRemove: ['toImage'],
          }}
          useResizeHandler
          style={{ width: '100%', height: '100%' }}
        />
      )}

      {displayData.length === 0 && !chartLoading && !chartError && (
        <Typography variant="caption" sx={{ color: 'text.secondary', py: 2 }}>
          {pandasConnected ? 'Waiting for live data...' : 'No decoded values found for this signal'}
        </Typography>
      )}
    </Box>
  );
}