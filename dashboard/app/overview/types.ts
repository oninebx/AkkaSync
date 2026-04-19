interface PipelineVM {
  name: string;
  runCount: number;
  totalProcessed: number;
  totalErrors: number;
  // schedule: string;
  // duration: string;
  // lastRun: string;
  // nextRun: string;
}

interface KpiVM {
  id: string;
  title: string;
  value: string;
  color: string;
}

export type {
  PipelineVM,
  KpiVM
}