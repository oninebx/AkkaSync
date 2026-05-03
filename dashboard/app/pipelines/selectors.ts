import { pipelineConfigSelectors, pipelineRuntimeSelectors } from "@/features/pipelines";
import { createSelector } from "@reduxjs/toolkit";
import { PipelineRow } from "./types";

const selectPipelines = pipelineConfigSelectors.selectEntities;
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
      nextRun: metrics[p.identifier].nextRun,
      errors: metrics[p.identifier].totalErrors
    }));
  }
);



export {
  selectPipelineRows
}