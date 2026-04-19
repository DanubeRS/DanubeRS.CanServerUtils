import { BrowserRouter, Routes, Route, Link as RouterLink } from 'react-router-dom';
import { AppBar, Toolbar, Typography, Button, Snackbar, Alert, Box, Chip } from '@mui/material';
import { UploadFile } from '@mui/icons-material';
import { useRef } from 'react';
import { useDbcStore } from './store';
import DbcEditorPage from './pages/DbcEditorPage';
import FrameViewerPage from './pages/FrameViewerPage';

function App() {
  const loaded = useDbcStore(s => s.loaded);
  const logLoaded = useDbcStore(s => s.logLoaded);
  const loading = useDbcStore(s => s.loading);
  const error = useDbcStore(s => s.error);
  const clearError = useDbcStore(s => s.clearError);
  const loadLog = useDbcStore(s => s.loadLog);
  const logFileRef = useRef<HTMLInputElement>(null);

  const handleLogFile = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) loadLog(file);
    e.target.value = '';
  };

  return (
    <BrowserRouter>
      <Box sx={{ display: 'flex', flexDirection: 'column', height: '100vh' }}>
        <AppBar position="static" elevation={1}>
          <Toolbar variant="dense">
            <Typography variant="h6" sx={{ flexGrow: 0, mr: 3, fontSize: '1rem' }}>
              DBC Web Editor
            </Typography>
            <Button color="inherit" component={RouterLink} to="/" size="small">Editor</Button>
            <Button color="inherit" component={RouterLink} to="/frames" size="small">Frames</Button>
            <Box sx={{ flexGrow: 1 }} />
            <Typography variant="caption" sx={{ mr: 1, opacity: 0.7 }}>
              {loading ? '⏳' : ''} {loaded ? '✓ DBC' : '— DBC'}
            </Typography>
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
          </Routes>
        </Box>
      </Box>
      <Snackbar open={!!error} autoHideDuration={8000} onClose={clearError} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
        <Alert severity="error" onClose={clearError} variant="filled">{error}</Alert>
      </Snackbar>
    </BrowserRouter>
  );
}

export default App;