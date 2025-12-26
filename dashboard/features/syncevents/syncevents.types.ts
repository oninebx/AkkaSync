export interface EventEnvelope {
  sequence: number;
  type: DashboardEventType;
  source: string;
  occurredAt: string;
}

export enum DashboardEventType {
  PipelineStarted = 'PipelineStarted',
  PipelineCompleted = 'PipelineCompleted'
}