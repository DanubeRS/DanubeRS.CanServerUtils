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
}

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
    set({ loading: true, error: null });
    try {
      await api.uploadLog(file);
      set({ logLoaded: true });
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
  },

  addSignal: async (frameId, create) => {
    await api.addSignal(frameId, create);
    await get().refreshMessages();
  },

  removeSignal: async (frameId, signalName) => {
    try {
      await api.removeSignal(frameId, signalName);
      await get().refreshMessages();
      const sel = get().selectedSignal;
      if (sel?.frameId === frameId && sel?.signalName === signalName)
        set({ selectedSignal: null });
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
}));