import { PipelineMetrics } from "@/features/pipelines/types";

type PipelineVM = PipelineMetrics

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