import { RootState } from "@/store";
import { pipelineAdapter } from "./pipeline.slice";

export const selectPipelineState = (state: RootState) => state.pipeline;

export const pipelineSelectors = pipelineAdapter.getSelectors(
  selectPipelineState
);

export const selectPipelines = pipelineSelectors.selectAll;
export const selectPipelineEntities = pipelineSelectors.selectEntities;
export const selectPipelineIds = pipelineSelectors.selectIds;

export const selectPipelineOverviewMap = (state: RootState) =>
  state.pipeline.overview;

export const selectPipelineLiveMap = (state: RootState) =>
  state.pipeline.live;

export const selectPipelineDefinitionMap = (state: RootState) =>
  state.pipeline.definition;