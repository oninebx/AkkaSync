export enum HostStatus {
  Syncing,
  Idle,
  Degraded,
  Stopped
}
export type StatusType = Lowercase<keyof typeof HostStatus>;

export interface PipelineSnapshot {
  id: string,
  startAt: string
}
export interface HostSnapshot {
  status: HostStatus;
  startAt: string;
  pipelines?: Record<string, PipelineSnapshot>;
}