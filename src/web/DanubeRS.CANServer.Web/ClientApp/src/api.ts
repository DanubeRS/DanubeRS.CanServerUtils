export interface SignalInfo {
  name: string;
  startBit: number;
  size: number;
  order: string;
  valueType: string;
  factor: number;
  offset: number;
  min: number;
  max: number;
  unit: string;
  receiver: string;
  multiplex: { isSwitch: boolean; switchValue: number | null } | null;
}

export interface MessageInfo {
  frameId: number;
  name: string;
  size: number;
  node: string;
  signals: SignalInfo[];
}

export interface FrameInfo {
  index: number;
  frameTime: number;
  frameId: number;
  busId: number;
  payload: number[];
  messageName: string | null;
}

export interface DecodedSignalValue {
  name: string;
  value: number;
  unit: string;
}

export interface DecodedFrame {
  messageName: string;
  multiplexValue: number | null;
  signals: DecodedSignalValue[];
}

export interface FrameDetail {
  index: number;
  frameTime: number;
  frameId: number;
  busId: number;
  payload: number[];
  decoded: DecodedFrame | null;
}

export interface SignalUpdate {
  name?: string;
  startBit?: number;
  size?: number;
  order?: string;
  valueType?: string;
  factor?: number;
  offset?: number;
  min?: number;
  max?: number;
  unit?: string;
  receiver?: string;
  multiplex?: { isSwitch: boolean; switchValue: number | null } | null;
  clearMultiplex?: boolean;
}

export interface SignalOverride {
  startBit?: number;
  size?: number;
  order?: string;
  valueType?: string;
  factor?: number;
  offset?: number;
  min?: number;
  max?: number;
  unit?: string;
}

export interface PreviewRequest {
  frameIndex: number;
  signalOverrides: Record<string, SignalOverride>;
}

export interface MessageCreate {
  frameId: number;
  name: string;
  size: number;
  node?: string;
}

export interface SignalCreate {
  name: string;
  startBit: number;
  size: number;
  order?: string;
  valueType?: string;
  factor?: number;
  offset?: number;
  min?: number;
  max?: number;
  unit?: string;
  receiver?: string;
  multiplex?: { isSwitch: boolean; switchValue: number | null } | null;
}

export interface UploadResult {
  messageCount?: number;
  signalCount?: number;
  frameCount?: number;
  distinctFrameIds?: number[];
  error?: string;
  details?: string;
}

const API_BASE = '/api';

async function apiFetch<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE}${url}`, options);
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ error: response.statusText }));
    throw new Error(errorData.error || errorData.details || `HTTP ${response.status}`);
  }
  return response.json();
}

async function apiFetchBlob(url: string, options?: RequestInit): Promise<Blob> {
  const response = await fetch(`${API_BASE}${url}`, options);
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ error: response.statusText }));
    throw new Error(errorData.error || errorData.details || `HTTP ${response.status}`);
  }
  return response.blob();
}

export async function loadDbc(file: File): Promise<UploadResult> {
  const formData = new FormData();
  formData.append('file', file);
  return apiFetch<UploadResult>('/dbc/load', { method: 'POST', body: formData });
}

export async function getMessages(): Promise<MessageInfo[]> {
  return apiFetch<MessageInfo[]>('/dbc/messages');
}

export async function updateSignal(frameId: number, signalName: string, update: SignalUpdate): Promise<SignalInfo> {
  return apiFetch<SignalInfo>(`/dbc/messages/${frameId}/signals/${signalName}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(update),
  });
}

export async function addSignal(frameId: number, create: SignalCreate): Promise<SignalInfo> {
  return apiFetch<SignalInfo>(`/dbc/messages/${frameId}/signals`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(create),
  });
}

export async function removeSignal(frameId: number, signalName: string): Promise<void> {
  await fetch(`${API_BASE}/dbc/messages/${frameId}/signals/${signalName}`, { method: 'DELETE' });
}

export async function addMessage(create: MessageCreate): Promise<MessageInfo> {
  return apiFetch<MessageInfo>('/dbc/messages', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(create),
  });
}

export async function removeMessage(frameId: number): Promise<void> {
  await fetch(`${API_BASE}/dbc/messages/${frameId}`, { method: 'DELETE' });
}

export async function exportDbc(): Promise<Blob> {
  return apiFetchBlob('/dbc/export');
}

export async function uploadLog(file: File): Promise<UploadResult> {
  const formData = new FormData();
  formData.append('file', file);
  return apiFetch<UploadResult>('/log/upload', { method: 'POST', body: formData });
}

export async function getFrames(skip: number, take: number, frameId?: number): Promise<FrameInfo[]> {
  const params = new URLSearchParams({ skip: String(skip), take: String(take) });
  if (frameId !== undefined) params.set('frameId', String(frameId));
  return apiFetch<FrameInfo[]>(`/log/frames?${params}`);
}

export async function getFrame(index: number): Promise<FrameDetail> {
  return apiFetch<FrameDetail>(`/log/frames/${index}`);
}

export async function previewDecode(request: PreviewRequest): Promise<DecodedFrame | null> {
  const result = await apiFetch<DecodedFrame | { decoded: null }>('/decode/preview', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if ('decoded' in result && result.decoded === null) return null;
  return result as DecodedFrame;
}

export interface TimeSeriesPoint {
  time: number;
  value: number;
}

export async function decodeSignalTimeSeries(frameId: number, signalName: string): Promise<TimeSeriesPoint[]> {
  return apiFetch<TimeSeriesPoint[]>(`/log/decode-signal?frameId=${frameId}&signalName=${encodeURIComponent(signalName)}`);
}