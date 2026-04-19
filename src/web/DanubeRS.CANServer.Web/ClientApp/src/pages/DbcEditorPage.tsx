import { Box, Divider } from '@mui/material';
import { SignalList } from '../components/SignalList';
import { SignalEditor } from '../components/SignalEditor';
import { LogSignalViewer } from '../components/LogSignalViewer';
import { useDbcStore } from '../store';

export default function DbcEditorPage() {
  const selectedSignal = useDbcStore(s => s.selectedSignal);

  return (
    <Box sx={{ display: 'flex', height: '100%', overflow: 'hidden' }}>
      <Box sx={{ width: 320, minWidth: 320, borderRight: 1, borderColor: 'divider', overflow: 'auto' }}>
        <SignalList />
      </Box>
      <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
        <Box sx={{ flex: 1, overflow: 'auto', p: 2 }}>
          {selectedSignal ? (
            <SignalEditor />
          ) : (
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%', opacity: 0.4 }}>
              Select a signal to edit
            </Box>
          )}
        </Box>
        {selectedSignal && (
          <>
            <Divider />
            <Box sx={{ height: 240, overflow: 'auto', p: 2 }}>
              <LogSignalViewer />
            </Box>
          </>
        )}
      </Box>
    </Box>
  );
}