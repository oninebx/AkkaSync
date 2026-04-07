interface WorkerRecord {
  id: string;
  pipelineId: string;
  startedAt: string;
  finishedAt: string;
  sourcePluginId: string;
  status: 'idle' | 'running' | 'succeeded' | 'failed';
  stats: {
    processed: number;
    errors: number;
  };
}

export type {
  WorkerRecord
}