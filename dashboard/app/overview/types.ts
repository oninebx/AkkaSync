interface PipelineVM {
  name: string;
  schedule: string;
  duration: string;
  lastRun: string;
  nextRun: string;
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