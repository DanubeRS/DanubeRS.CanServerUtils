import { useState, useEffect, useRef } from 'react';
import { Box, Slider, IconButton, Typography, Stack } from '@mui/material';
import { PlayArrow, Pause } from '@mui/icons-material';
import { useDbcStore } from '../store';
import type { RawFrameInfo } from '../api';

const SPEED_OPTIONS = [
  { label: '0.5x', interval: 2000 },
  { label: '1x', interval: 1000 },
  { label: '2x', interval: 500 },
  { label: '5x', interval: 200 },
];

interface PlaybackControlsProps {
  frames: RawFrameInfo[];
  liveMode?: boolean;
}

export function PlaybackControls({ frames, liveMode }: PlaybackControlsProps) {
  const analysisIndex = useDbcStore(s => s.analysisPlaybackIndex);
  const setAnalysisIndex = useDbcStore(s => s.setAnalysisPlaybackIndex);
  const liveIndex = useDbcStore(s => s.livePlaybackIndex);
  const setLiveIndex = useDbcStore(s => s.setLivePlaybackIndex);

  const playbackIndex = liveMode ? liveIndex : analysisIndex;
  const setPlaybackIndex = liveMode ? setLiveIndex : setAnalysisIndex;
  const [playing, setPlaying] = useState(false);
  const [speedIdx, setSpeedIdx] = useState(1);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    if (playing) {
      timerRef.current = setInterval(() => {
        setPlaybackIndex(playbackIndex + 1);
      }, SPEED_OPTIONS[speedIdx].interval);
    } else if (timerRef.current) {
      clearInterval(timerRef.current);
      timerRef.current = null;
    }
    return () => {
      if (timerRef.current) {
        clearInterval(timerRef.current);
        timerRef.current = null;
      }
    };
  }, [playing, speedIdx, playbackIndex, setPlaybackIndex]);

  useEffect(() => {
    if (playbackIndex >= frames.length - 1) {
      setPlaying(false);
    }
  }, [playbackIndex, frames.length]);

  const handleSliderChange = (_: Event, value: number | number[]) => {
    setPlaybackIndex(value as number);
  };

  if (frames.length === 0) return null;

  return (
    <Box sx={{ mt: 2 }}>
      <Typography variant="caption" color="text.secondary">
        Frame {playbackIndex + 1} / {frames.length}
      </Typography>
      <Slider
        value={playbackIndex}
        onChange={handleSliderChange}
        min={0}
        max={Math.max(0, frames.length - 1)}
        step={1}
        size="small"
        sx={{ mt: 0.5 }}
      />
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', justifyContent: 'center' }}>
        <IconButton size="small" onClick={() => setPlaying(!playing)}>
          {playing ? <Pause fontSize="small" /> : <PlayArrow fontSize="small" />}
        </IconButton>
        <Stack direction="row" spacing={0.5}>
          {SPEED_OPTIONS.map((s, i) => (
            <Typography
              key={s.label}
              variant="caption"
              onClick={() => setSpeedIdx(i)}
              sx={{
                cursor: 'pointer',
                px: 0.5,
                borderRadius: 1,
                bgcolor: i === speedIdx ? 'primary.main' : 'transparent',
                color: i === speedIdx ? 'primary.contrastText' : 'text.secondary',
              }}
            >
              {s.label}
            </Typography>
          ))}
        </Stack>
      </Stack>
    </Box>
  );
}