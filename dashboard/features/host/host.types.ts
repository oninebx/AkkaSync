export enum HostStatus {
  Syncing,
  Idle,
  Degraded,
  Stopped
}

export interface HostSnapshot {
  status: HostStatus;
  startAt: string;
}

export type StatusType = Lowercase<keyof typeof HostStatus>;