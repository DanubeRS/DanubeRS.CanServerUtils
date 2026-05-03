import { BrowserRouter, Routes, Route, Link as RouterLink } from 'react-router-dom';
import { AppBar, Toolbar, Typography, Button, Snackbar, Alert, Box, Chip, IconButton, Tooltip, Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, ThemeProvider, createTheme, CssBaseline } from '@mui/material';
import { UploadFile, DarkMode, LightMode, SettingsBrightness, Cable, PauseCircleOutlined, PlayCircleOutlined, DeleteOutlined } from '@mui/icons-material';
import { useRef, useState, useMemo } from 'react';
import { useDbcStore } from './store';
import { useThemeMode } from './hooks/useThemeMode';
import { usePandasWebSocket } from './hooks/usePandasWebSocket';
import { PandasConnectDialog } from './components/PandasConnectDialog';
import DbcEditorPage from './pages/DbcEditorPage';
import FrameViewerPage from './pages/FrameViewerPage';
import BitAnalyzerPage from './pages/BitAnalyzerPage';

function App() {
  const loaded = useDbcStore(s => s.loaded);
  const logLoaded = useDbcStore(s => s.logLoaded);
  const pandasConnected = useDbcStore(s => s.pandasConnected);
  const loading = useDbcStore(s => s.loading);
  const error = useDbcStore(s => s.error);
  const clearError = useDbcStore(s => s.clearError);
  const loadLog = useDbcStore(s => s.loadLog);
  const disconnectPandas = useDbcStore(s => s.disconnectPandas);
  const capturing = useDbcStore(s => s.capturing);
  const toggleCapturing = useDbcStore(s => s.toggleCapturing);
  const clearLiveHistory = useDbcStore(s => s.clearLiveHistory);
  const logFileRef = useRef<HTMLInputElement>(null);
  const { mode, preference, cyclePreference } = useThemeMode();
  const [pandasDialogOpen, setPandasDialogOpen] = useState(false);
  const [disconnectConfirmOpen, setDisconnectConfirmOpen] = useState(false);

  usePandasWebSocket();

  const theme = useMemo(() => createTheme({
    palette: { mode },
    typography: { fontFamily: '"Inter", system-ui, sans-serif' },
  }), [mode]);

  const handleLogFile = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) loadLog(file);
    e.target.value = '';
  };

  const themeIcon = preference === 'light' ? <LightMode /> : preference === 'dark' ? <DarkMode /> : <SettingsBrightness />;
  const themeLabel = preference === 'light' ? 'Light mode' : preference === 'dark' ? 'Dark mode' : 'System theme';

  const handleLiveChipClick = () => {
    if (pandasConnected) {
      setDisconnectConfirmOpen(true);
    } else {
      setPandasDialogOpen(true);
    }
  };

  const handleDisconnectConfirm = async () => {
    await disconnectPandas();
    setDisconnectConfirmOpen(false);
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Box sx={{ display: 'flex', flexDirection: 'column', height: '100vh' }}>
          <AppBar position="static" elevation={1}>
            <Toolbar variant="dense">
              <Typography variant="h6" sx={{ flexGrow: 0, mr: 3, fontSize: '1rem' }}>
                DBC Web Editor
              </Typography>
              <Button color="inherit" component={RouterLink} to="/" size="small">Editor</Button>
              <Button color="inherit" component={RouterLink} to="/frames" size="small">Frames</Button>
              <Button color="inherit" component={RouterLink} to="/bits" size="small">Bits</Button>
              <Box sx={{ flexGrow: 1 }} />
              <Tooltip title={themeLabel}>
                <IconButton color="inherit" onClick={cyclePreference} size="small">
                  {themeIcon}
                </IconButton>
              </Tooltip>
              <Typography variant="caption" sx={{ mr: 1, opacity: 0.7 }}>
                {loading ? '⏳' : ''} {loaded ? '✓ DBC' : '— DBC'}
              </Typography>
              <Chip
                icon={<Cable fontSize="small" />}
                label={pandasConnected ? 'Live' : '— Live'}
                size="small"
                color={pandasConnected ? 'success' : 'default'}
                variant={pandasConnected ? 'filled' : 'outlined'}
                onClick={handleLiveChipClick}
                sx={{ cursor: 'pointer', mr: 1 }}
              />
              {pandasConnected && (
                <>
                  <Tooltip title={capturing ? 'Pause capture' : 'Resume capture'}>
                    <IconButton color="inherit" onClick={toggleCapturing} size="small">
                      {capturing ? <PauseCircleOutlined /> : <PlayCircleOutlined />}
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Clear captured data">
                    <IconButton color="inherit" onClick={clearLiveHistory} size="small">
                      <DeleteOutlined />
                    </IconButton>
                  </Tooltip>
                </>
              )}
              <Chip
                icon={<UploadFile fontSize="small" />}
                label={logLoaded ? '✓ Log' : '— Log'}
                size="small"
                variant={logLoaded ? 'filled' : 'outlined'}
                onClick={() => logFileRef.current?.click()}
                sx={{ cursor: 'pointer' }}
              />
              <input ref={logFileRef} type="file" accept=".bin,.log" onChange={handleLogFile} hidden />
            </Toolbar>
          </AppBar>
          <Box sx={{ flexGrow: 1, overflow: 'hidden' }}>
            <Routes>
              <Route path="/" element={<DbcEditorPage />} />
              <Route path="/frames" element={<FrameViewerPage />} />
              <Route path="/bits" element={<BitAnalyzerPage />} />
            </Routes>
          </Box>
        </Box>
        <PandasConnectDialog open={pandasDialogOpen} onClose={() => setPandasDialogOpen(false)} />
        <Dialog open={disconnectConfirmOpen} onClose={() => setDisconnectConfirmOpen(false)}>
          <DialogTitle>Disconnect from Panda?</DialogTitle>
          <DialogContent>
            <DialogContentText>Live data streaming will stop and all captured signal data will be discarded.</DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDisconnectConfirmOpen(false)}>Cancel</Button>
            <Button onClick={handleDisconnectConfirm} color="error" variant="contained">Disconnect</Button>
          </DialogActions>
        </Dialog>
        <Snackbar open={!!error} autoHideDuration={8000} onClose={clearError} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
          <Alert severity="error" onClose={clearError} variant="filled">{error}</Alert>
        </Snackbar>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;