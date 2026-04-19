import { useEffect, useRef, useCallback } from 'react';
import { useDbcStore } from '../store';
import * as api from '../api';

export function usePandasWebSocket() {
  const wsRef = useRef<WebSocket | null>(null);
  const trackRef = useRef<Set<number>>(new Set());
  const pandasConnected = useDbcStore(s => s.pandasConnected);
  const selectedSignal = useDbcStore(s => s.selectedSignal);
  const connect = useCallback(() => {
    if (wsRef.current) {
      wsRef.current.close();
      wsRef.current = null;
    }

    const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
    const wsUrl = `${protocol}//${window.location.host}/ws/pandas`;
    console.log('[PandasWS] Connecting to', wsUrl);
    const ws = new WebSocket(wsUrl);

    ws.onopen = () => {
      console.log('[PandasWS] Connected');
      useDbcStore.getState().setPandasConnected(true);

      const trackedFrames = [...trackRef.current];
      if (trackedFrames.length > 0) {
        ws.send(JSON.stringify({ type: 'track', frameIds: trackedFrames }));
      }
    };

    ws.onmessage = (event) => {
      try {
        const msg = JSON.parse(event.data);
        if (msg.type === 'frame') {
          useDbcStore.getState().addLiveFrame(msg as api.WsFrameMessage);
        } else if (msg.type === 'status') {
          useDbcStore.getState().setPandasConnected(msg.connected);
          if (!msg.connected) {
            useDbcStore.getState().clearLiveData();
          }
        }
      } catch (e) {
        console.error('[PandasWS] Parse error:', e);
      }
    };

    ws.onclose = () => {
      console.log('[PandasWS] Disconnected');
      useDbcStore.getState().setPandasConnected(false);
    };

    ws.onerror = (e) => {
      console.error('[PandasWS] Error:', e);
    };

    wsRef.current = ws;
  }, []);

  const disconnect = useCallback(() => {
    if (wsRef.current) {
      wsRef.current.close();
      wsRef.current = null;
    }
    trackRef.current.clear();
  }, []);

  const track = useCallback((frameIds: number[]) => {
    const ws = wsRef.current;
    if (!ws || ws.readyState !== WebSocket.OPEN) return;
    for (const id of frameIds) {
      trackRef.current.add(id);
    }
    ws.send(JSON.stringify({ type: 'track', frameIds }));
  }, []);

  const untrack = useCallback((frameIds: number[]) => {
    const ws = wsRef.current;
    if (!ws || ws.readyState !== WebSocket.OPEN) return;
    for (const id of frameIds) {
      trackRef.current.delete(id);
    }
    ws.send(JSON.stringify({ type: 'untrack', frameIds }));
  }, []);

  useEffect(() => {
    if (pandasConnected && (!wsRef.current || wsRef.current.readyState === WebSocket.CLOSED)) {
      connect();
    }
  }, [pandasConnected, connect]);

  useEffect(() => {
    if (!pandasConnected || !selectedSignal) return;
    const frameId = selectedSignal.frameId;
    useDbcStore.getState().clearLiveData();
    track([frameId]);
    return () => {
      untrack([frameId]);
    };
  }, [pandasConnected, selectedSignal?.frameId, selectedSignal?.signalName, track, untrack]);

  useEffect(() => {
    return () => {
      if (wsRef.current) {
        wsRef.current.close();
        wsRef.current = null;
      }
    };
  }, []);

  return { connect, disconnect, track, untrack };
}