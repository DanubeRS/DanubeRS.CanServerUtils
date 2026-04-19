import { useState, useEffect, useCallback } from 'react';

type ThemeMode = 'light' | 'dark';
type ThemePreference = 'light' | 'dark' | 'system';

const STORAGE_KEY = 'theme-mode';

function getSystemPreference(): ThemeMode {
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

function resolveMode(preference: ThemePreference): ThemeMode {
  if (preference === 'system') return getSystemPreference();
  return preference;
}

export function useThemeMode() {
  const [preference, setPreference] = useState<ThemePreference>(() => {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored === 'light' || stored === 'dark' || stored === 'system') return stored;
    return 'system';
  });

  const [mode, setMode] = useState<ThemeMode>(() => resolveMode(preference));

  useEffect(() => {
    setMode(resolveMode(preference));
  }, [preference]);

  useEffect(() => {
    if (preference !== 'system') return;
    const mql = window.matchMedia('(prefers-color-scheme: dark)');
    const handler = () => setMode(getSystemPreference());
    mql.addEventListener('change', handler);
    return () => mql.removeEventListener('change', handler);
  }, [preference]);

  const toggleMode = useCallback(() => {
    setPreference(prev => {
      let next: ThemePreference;
      if (prev === 'system') next = getSystemPreference() === 'light' ? 'dark' : 'light';
      else next = prev === 'light' ? 'dark' : 'light';
      localStorage.setItem(STORAGE_KEY, next);
      return next;
    });
  }, []);

  const cyclePreference = useCallback(() => {
    setPreference(prev => {
      const next: ThemePreference = prev === 'light' ? 'dark' : prev === 'dark' ? 'system' : 'light';
      localStorage.setItem(STORAGE_KEY, next);
      return next;
    });
  }, []);

  return { mode, preference, toggleMode, cyclePreference };
}