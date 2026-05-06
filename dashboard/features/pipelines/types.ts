import { PluginKind } from "@/contracts/plugin/types";

interface PluginInfo {
  provider: string;
  kind: PluginKind;
}

enum PipelineStatus {
  UNKNOWN = 0,
  RUNNING = 1,
  SUCCESS = 2,
  FAILED = 3,
  RETRYING = 4,
  SKIPPED = 5,
}

interface PipelineDefinition {
  identifier: string;
  name: string;
  schedule: string | null;
  sourceId: string;
  sinkIds: string[];
  plugins: Record<string, PluginInfo>;
}

interface PipelineMetrics {
  identifier: string;
  name: string;
  totalRuns: number;
  totalProcessed: number;
  totalErrors: number;
  nextRun: string;
  status: PipelineStatus;
}

export type {
  PipelineDefinition,
  PipelineMetrics,
  PluginInfo
}

export {
  PipelineStatus
}

