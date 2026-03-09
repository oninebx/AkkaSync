import { RootState } from "@/store";
import { pipelineAdapter } from "./pipeline.slice";

export const selectPipelineState = (state: RootState) => state.pipeline;

export const pipelineSelectors = pipelineAdapter.getSelectors(
  selectPipelineState
);

export const selectPipelines = pipelineSelectors.selectAll;
export const selectPipelineEntities = pipelineSelectors.selectEntities;
export const selectPipelineIds = pipelineSelectors.selectIds;