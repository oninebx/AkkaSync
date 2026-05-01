import { PluginKind } from "@/contracts/plugin/types";
import { IBusinessState } from "@/types";
import { createEntityAdapter, createSlice } from "@reduxjs/toolkit";

interface PluginInfo {
  provider: string;
  kind: PluginKind;
}

interface PipelineDefinition {
  identifier: string;
  name: string;
  schedule: string | null;
  sourceId: string;
  targetIds: string[];
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

