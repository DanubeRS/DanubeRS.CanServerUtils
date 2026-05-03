import { create } from 'zustand';
import * as api from './api';

interface AppState {
  loaded: boolean;
  logLoaded: boolean;
  loading: boolean;
  messages: api.MessageInfo[];
  frames: api.FrameInfo[];
  selectedFrameId: number | null;
  selectedSignal: { frameId: number; signalName: string } | null;
  totalFrameCount: number;
  error: string | null;
  signalVersion: number;

  pandasConnected: boolean;
  pandasIp: string | null;
  pandasPort: number | null;
  liveSignalData: Map<string, { time: number; value: number }[]>;
  liveFrameIds: number[];
  liveVersion: number;

  loadDbc: (file: File) => Promise<void>;
  loadLog: (file: File) => Promise<void>;
  refreshMessages: () => Promise<void>;
  selectFrameId: (id: number | null) => void;
  selectSignal: (frameId: number, signalName: string) => void;
  clearSelectedSignal: () => void;
  updateSignal: (frameId: number, signalName: string, update: api.SignalUpdate) => Promise<void>;
  addSignal: (frameId: number, create: api.SignalCreate) => Promise<void>;
  removeSignal: (frameId: number, signalName: string) => Promise<void>;
  addMessage: (create: api.MessageCreate) => Promise<void>;
  removeMessage: (frameId: number) => Promise<void>;
  loadFrames: (skip: number, take: number, frameId?: number) => Promise<void>;
  exportDbc: () => Promise<Blob>;
  clearError: () => void;

  connectPandas: (ip: string, port: number) => Promise<void>;
  disconnectPandas: () => Promise<void>;
  setPandasConnected: (connected: boolean, ip?: string | null, port?: number | null) => void;
  addLiveFrame: (frame: api.WsFrameMessage) => void;
  addLiveAnalysisFrame: (frame: api.WsRawFrameMessage) => void;
  clearLiveData: () => void;
  clearLiveSignals: () => void;

  analysisFrames: Map<number, api.RawFrameInfo[]>;
  analysisWindow: number;
  analysisPlaybackIndex: number;
  liveFrameHistory: Map<number, api.RawFrameInfo[]>;
  liveFrameHistoryVersion: number;
  livePlaybackIndex: number;
  capturing: boolean;
  loadAnalysisFrames: (frameId: number) => Promise<void>;
  clearAnalysisData: () => void;
  setAnalysisPlaybackIndex: (index: number) => void;
  setLivePlaybackIndex: (index: number) => void;
  toggleCapturing: () => void;
  clearLiveHistory: () => void;
}

const MAX_LIVE_SECONDS = 60;

export const useDbcStore = create<AppState>((set, get) => ({
  loaded: false,
  logLoaded: false,
  loading: false,
  messages: [],
  frames: [],
  selectedFrameId: null,
  selectedSignal: null,
  totalFrameCount: 0,
  error: null,
  signalVersion: 0,
  pandasConnected: false,
  pandasIp: null,
  pandasPort: null,
  liveSignalData: new Map<string, { time: number; value: number }[]>(),
  liveFrameIds: [],
  liveVersion: 0,
  analysisFrames: new Map<number, api.RawFrameInfo[]>(),
  analysisWindow: 100,
  analysisPlaybackIndex: 0,
  liveFrameHistory: new Map<number, api.RawFrameInfo[]>(),
  liveFrameHistoryVersion: 0,
  livePlaybackIndex: 0,
  capturing: true,

  loadDbc: async (file: File) => {
    set({ loading: true, error: null });
    try {
      await api.loadDbc(file);
      set({ loaded: true });
      try { await get().refreshMessages(); } catch (e: any) { console.error('Refresh messages failed:', e); }
    } catch (e: any) {
      console.error('DBC load failed:', e);
      set({ error: e.message });
    } finally {
      set({ loading: false });
    }
  },

  loadLog: async (file: File) => {
    if (get().pandasConnected) {
      await get().disconnectPandas();
    }
    set({ loading: true, error: null });
    try {
      await api.uploadLog(file);
      set({ logLoaded: true, liveSignalData: new Map() });
      try { await get().loadFrames(0, 100); } catch (e: any) { console.error('Load frames failed:', e); }
    } catch (e: any) {
      console.error('Log upload failed:', e);
      set({ error: e.message });
    } finally {
      set({ loading: false });
    }
  },

  refreshMessages: async () => {
    try {
      const messages = await api.getMessages();
      set({ messages });
    } catch (e: any) {
      console.error('Refresh messages failed:', e);
      set({ error: e.message });
    }
  },

  selectFrameId: (id) => set({ selectedFrameId: id }),
  selectSignal: (frameId, signalName) => set({ selectedSignal: { frameId, signalName } }),
  clearSelectedSignal: () => set({ selectedSignal: null }),

  updateSignal: async (frameId, signalName, update) => {
    await api.updateSignal(frameId, signalName, update);
    await get().refreshMessages();
    const newName = (update as Record<string, unknown>).name as string | undefined;
    if (newName && newName !== signalName) {
      set({ selectedSignal: { frameId, signalName: newName } });
    }
    set(s => ({ signalVersion: s.signalVersion + 1 }));
  },

  addSignal: async (frameId, create) => {
    await api.addSignal(frameId, create);
    await get().refreshMessages();
    set(s => ({ signalVersion: s.signalVersion + 1 }));
  },

  removeSignal: async (frameId, signalName) => {
    try {
      await api.removeSignal(frameId, signalName);
      await get().refreshMessages();
      const sel = get().selectedSignal;
      if (sel?.frameId === frameId && sel?.signalName === signalName)
        set({ selectedSignal: null });
      set(s => ({ signalVersion: s.signalVersion + 1 }));
    } catch (e: any) {
      console.error('Remove signal failed:', e);
      set({ error: e.message });
    }
  },

  addMessage: async (create) => {
    await api.addMessage(create);
    await get().refreshMessages();
  },

  removeMessage: async (frameId) => {
    try {
      await api.removeMessage(frameId);
      await get().refreshMessages();
    } catch (e: any) {
      console.error('Remove message failed:', e);
      set({ error: e.message });
    }
  },

  loadFrames: async (skip, take, frameId) => {
    try {
      const frames = await api.getFrames(skip, take, frameId);
      set({ frames });
    } catch (e: any) {
      console.error('Load frames failed:', e);
      set({ error: e.message });
    }
  },

  exportDbc: async () => {
    return api.exportDbc();
  },

  clearError: () => set({ error: null }),

  connectPandas: async (ip: string, port: number) => {
    try {
      const result = await api.pandasConnect(ip, port);
      set({
        pandasConnected: result.connected,
        pandasIp: result.connected ? ip : null,
        pandasPort: result.connected ? port : null,
        logLoaded: false,
        frames: [],
        totalFrameCount: 0,
        liveSignalData: new Map(),
        liveFrameIds: [],
        liveVersion: 0,
  analysisFrames: new Map(),
  analysisWindow: 100,
  analysisPlaybackIndex: 0,
   liveFrameHistory: new Map(),
   liveFrameHistoryVersion: 0,
   livePlaybackIndex: 0,
   capturing: true,
      });
    } catch (e: any) {
      console.error('Pandas connect failed:', e);
      set({ error: e.message, pandasConnected: false, pandasIp: null, pandasPort: null });
    }
  },

  disconnectPandas: async () => {
    try {
      await api.pandasDisconnect();
    } catch (e: any) {
      console.error('Pandas disconnect failed:', e);
    }
    set({ pandasConnected: false, pandasIp: null, pandasPort: null, liveSignalData: new Map(), liveFrameIds: [], liveVersion: 0, liveFrameHistory: new Map(), liveFrameHistoryVersion: 0, livePlaybackIndex: 0, capturing: true });
  },

  setPandasConnected: (connected: boolean, ip?: string | null, port?: number | null) => {
    set({
      pandasConnected: connected,
      pandasIp: connected ? (ip ?? get().pandasIp) : null,
      pandasPort: connected ? (port ?? get().pandasPort) : null,
    });
  },

  addLiveFrame: (frame: api.WsFrameMessage) => {
    const { liveSignalData, liveFrameIds, capturing } = get();
    if (!capturing) return;
    const cutoff = frame.timestamp - MAX_LIVE_SECONDS * 1000;
    for (const sig of frame.signals) {
      const key = `${frame.frameId}:${sig.name}`;
      const arr = liveSignalData.get(key);
      if (arr) {
        const pruneIdx = arr.findIndex(p => p.time >= cutoff);
        if (pruneIdx > 0) arr.splice(0, pruneIdx);
        arr.push({ time: frame.timestamp, value: sig.value });
      } else {
        liveSignalData.set(key, [{ time: frame.timestamp, value: sig.value }]);
      }
    }
    const newFrameIds = liveFrameIds.includes(frame.frameId) ? liveFrameIds : [...liveFrameIds, frame.frameId];
    set(s => ({ liveVersion: s.liveVersion + 1, liveFrameIds: newFrameIds }));
  },

  clearLiveData: () => set({ liveSignalData: new Map(), liveFrameIds: [], liveVersion: 0, liveFrameHistory: new Map(), liveFrameHistoryVersion: 0, livePlaybackIndex: 0, capturing: true }),
  clearLiveSignals: () => set({ liveSignalData: new Map(), liveVersion: 0 }),

  loadAnalysisFrames: async (frameId: number) => {
    try {
      const frames = await api.getRawFrames(frameId, 0, get().analysisWindow);
      const newMap = new Map(get().analysisFrames);
      newMap.set(frameId, frames);
      set({ analysisFrames: newMap, analysisPlaybackIndex: 0 });
    } catch (e: any) {
      console.error('Load analysis frames failed:', e);
      set({ error: e.message });
    }
  },

  addLiveAnalysisFrame: (frame: api.WsRawFrameMessage) => {
    const { analysisFrames, analysisWindow, liveFrameIds, liveFrameHistory, capturing } = get();
    if (!capturing) return;
    const rawFrame: api.RawFrameInfo = {
      frameTime: frame.timestamp,
      frameId: frame.frameId,
      busId: 0,
      payload: frame.rawData,
    };
    const frames = analysisFrames.get(frame.frameId);
    if (frames) {
      frames.push(rawFrame);
      if (frames.length > analysisWindow) frames.splice(0, frames.length - analysisWindow);
    } else {
      analysisFrames.set(frame.frameId, [rawFrame]);
    }
    const history = liveFrameHistory.get(frame.frameId);
    if (history) {
      history.push(rawFrame);
      if (history.length > 100) history.splice(0, history.length - 100);
    } else {
      liveFrameHistory.set(frame.frameId, [rawFrame]);
    }
    const newFrameIds = liveFrameIds.includes(frame.frameId) ? liveFrameIds : [...liveFrameIds, frame.frameId];
    set({ analysisFrames: new Map(analysisFrames), liveFrameIds: newFrameIds, liveFrameHistory: new Map(liveFrameHistory), liveFrameHistoryVersion: get().liveFrameHistoryVersion + 1 });
  },

  clearAnalysisData: () => set({ analysisFrames: new Map(), analysisPlaybackIndex: 0 }),

  setAnalysisPlaybackIndex: (index: number) => set({ analysisPlaybackIndex: index }),
  setLivePlaybackIndex: (index: number) => set({ livePlaybackIndex: index }),
  toggleCapturing: () => set(s => ({ capturing: !s.capturing })),
  clearLiveHistory: () => set({ liveFrameHistory: new Map(), liveFrameHistoryVersion: 0, livePlaybackIndex: 0, liveSignalData: new Map(), liveFrameIds: [], liveVersion: 0 }),
}));