import { PipelineStatus } from "@/features/pipelines/types";

type PipelineRowStatus = keyof typeof PipelineStatus;

interface PipelineRow {
  id: string;
  name: string;
  runs: number;
  processed: number;
  scheduleText: string;
  status: PipelineRowStatus;
  lastRun: string;
  nextRun: string;
  errors: number;
};

export type {
  PipelineRow,
  PipelineRowStatus
}