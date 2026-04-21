import { RootState } from "@/store";
import { pipelineAdapter, runAdapter } from "./pipeline.slice";

export const selectPipelineBaseState = (state: RootState) => state.pipeline.base;

export const pipelineBaseSelectors = pipelineAdapter.getSelectors(selectPipelineBaseState);
export const selectBasePipelines = pipelineBaseSelectors.selectAll;
export const selectBasePipelineEntities = pipelineBaseSelectors.selectEntities;
export const selectBasePipelineIds = pipelineBaseSelectors.selectIds;

export const selectPipelineOverviewMap = (state: RootState) =>
  state.pipeline.base.overview;

export const selectPipelineDefinitionMap = (state: RootState) =>
  state.pipeline.base.definition;


export const selectPipelineRunState = (state: RootState) => state.pipeline.run;
export const pipelineRunSelectors = runAdapter.getSelectors(selectPipelineRunState);
export const selectPipelineRuns = pipelineRunSelectors.selectAll;
export const selectPipelineRunEntities = pipelineRunSelectors.selectEntities;
export const selectPipelineRunMap = (state: RootState) => state.pipeline.run.byPipelinePlugin;