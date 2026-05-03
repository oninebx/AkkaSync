import { PluginKind } from "@/contracts/plugin/types";

interface PluginInfo {
  provider: string;
  kind: PluginKind;
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
}

export type {
  PipelineDefinition,
  PipelineMetrics,
  PluginInfo
}

