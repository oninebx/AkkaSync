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

import { PluginType } from "@/contracts/plugin/types";

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

interface PipelineLive {
  id: string;
  plugins: PluginLive[];
}

interface PluginLive {
  id: string;
}

export type {
  PipelineBase,
  PipelineOverview,
  DataSource,
  PipelineDefinition,
  PluginDefinition,
  PipelineLive,
  PluginLive
}