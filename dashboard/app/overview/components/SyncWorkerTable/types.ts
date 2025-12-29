type SyncWorkerStatus = 'success' | 'warning' | 'error' | 'running';

interface OverviewSyncWorker {
  name: string;
  runId: string;
  status: SyncWorkerStatus;
  stage: string;
  progress: number;
}

export type {
  SyncWorkerStatus,
  OverviewSyncWorker
}