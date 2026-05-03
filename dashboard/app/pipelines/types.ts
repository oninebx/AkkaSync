interface PipelineRow {
  id: string;
  name: string;
  runs: number;
  processed: number;
  scheduleText: string;
  status: 'RUNNING' | 'FAILED' | 'SUCCESS' | 'IDLE';
  lastRun: string;
  errors: number;
};

export type {
  PipelineRow
}