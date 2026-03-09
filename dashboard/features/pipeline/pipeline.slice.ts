import { createEntityAdapter, createSlice } from "@reduxjs/toolkit";
import { Pipeline } from "./pipeline.types";
import { RootState } from "@/store";

export const pipelineAdapter = createEntityAdapter<Pipeline>();

const pipelineSlice = createSlice({
  name: 'pipeline',
  initialState: pipelineAdapter.getInitialState(),
  reducers: {
    setPipelines: pipelineAdapter.setAll,
    pipelineAdded: pipelineAdapter.addOne,
    pipelineUpdated: pipelineAdapter.upsertOne,
    pipelineRemoved: pipelineAdapter.removeOne
  }
});

export const { setPipelines, pipelineAdded, pipelineUpdated, pipelineRemoved } = pipelineSlice.actions;
export default pipelineSlice.reducer;
export const pipelineSelectors = pipelineAdapter.getSelectors((state: RootState) => state.pipeline);
