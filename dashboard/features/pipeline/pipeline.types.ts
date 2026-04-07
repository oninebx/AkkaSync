interface Pipeline {
  id: string;
  name: string;
  schedule: string;
  nextRunAt: string;
  lastRunAt?: string;
  lastSuccessAt?: string;
  stats: {
    totalRuns: number;
    successfulRuns: number;
    failedRuns: number;
    totalProcessed: number;
    totalErrors: number;
  };

}

export type {
  Pipeline
}