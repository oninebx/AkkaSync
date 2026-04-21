// interface Pipeline {
//   id: string;
//   name: string;
//   schedule: string;
//   nextRunAt: string;
//   lastRunAt?: string;
//   lastSuccessAt?: string;
//   stats: {
//     totalRuns: number;
//     successfulRuns: number;
//     failedRuns: number;
//     totalProcessed: number;
//     totalErrors: number;
//   };

import { PluginStatus, PluginType } from "@/contracts/plugin/types";

// }

// export type {
//   Pipeline
// }

interface PipelineBase {
  id: string;
  name: string;
}

interface PipelineOverview {
  id: string;
  runCount: number;
  totalProcessed: number;
  totalErrors: number;
}

interface DataSource {
  id: string;
  name: string;
  type: string;
}

interface PipelineDefinition {
  id: string;
  name: string;
  source: DataSource;
  target: DataSource;
  schedule: string;
  plugins: PluginDefinition[];
}

interface PluginDefinition {
  key: string;
  name: string;
  type: PluginType;
  dependsOn?: string[];
}

interface PipelineRun {
  id: string;
  plugins: Record<string, PluginRun>;
}

interface PluginRun {
  id: string;
  key: string;
  name: string;
  status: PluginStatus;
  dependsOn: string[];
  processed: number;
  errors: number;
}

export type {
  PipelineBase,
  PipelineOverview,
  DataSource,
  PipelineDefinition,
  PluginDefinition,
  PipelineRun,
  PluginRun
}