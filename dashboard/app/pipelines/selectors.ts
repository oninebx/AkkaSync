import { pipelineConfigSelectors, pipelineRuntimeSelectors } from "@/features/pipelines";
import { createSelector } from "@reduxjs/toolkit";
import cronstrue from "cronstrue";
import { PipelineRow } from "./types";
import { connectorConfigSelectors } from "@/features/connector";
import { pluginCacheSelectors, pluginCentralSelectors, pluginConfigSelectors } from "@/features/plugins";

const selectPipelines = pipelineConfigSelectors.selectEntities;
const selectConnectors = connectorConfigSelectors.selectEntities;
const selectPlugins = pluginConfigSelectors.selectAll;
const selectCachePlugins = pluginCacheSelectors.selectEntities;
const selectCentralPlugins = pluginCentralSelectors.selectEntities;

const selectMetrics = pipelineRuntimeSelectors.selectEntities;


const selectPipelineRows = createSelector(
  [ selectPipelines, selectMetrics],
  (pipelines, metrics): PipelineRow[] => {
    return Object.values(pipelines).map(p => ({
      id: p.identifier,
      name: p.name,
      runs: metrics[p.identifier].totalRuns,
      processed: metrics[p.identifier].totalProcessed,
      // scheduleText: p.schedule ? cronstrue.toString(p.schedule) : 'Manual',
      scheduleText: 'Manual',
      status: 'IDLE',
      lastRun: 'Never',
      errors: metrics[p.identifier].totalErrors
    }));
  }
);

const selectPipelineTopology = createSelector(
  [
    selectPipelines,
    selectConnectors,
    selectPlugins,
    selectCachePlugins,
    selectCentralPlugins,
    (_, pipelineId: string) => pipelineId
  ],
  (definitions, connectors, id) => {
    return id;
});

export {
  selectPipelineRows,
  selectPipelineTopology
}