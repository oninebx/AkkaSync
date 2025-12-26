export enum HostStatus {
  Syncing,
  Idle,
  Degraded,
  Stopped
}

export interface PipelineSnapshot {
  id: string,
  startAt: string
}

export interface HostSnapshot {
  status: HostStatus;
  pipelinesTotal: number;
  startAt: string;
  pipelines?: PipelineSnapshot[];
}

export type StatusType = Lowercase<keyof typeof HostStatus>;