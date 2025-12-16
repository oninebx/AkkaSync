export enum HostStatus {
  Online,
  Offline
}

export type EngineStatus = 'starting' | 'syncing' | 'idle' | 'paused' | 'stopped' | 'failed';
export type StatusType = Lowercase<keyof typeof HostStatus> | EngineStatus;

export interface HostSnapshot {
  status: HostStatus;
  timestamp: string;
}