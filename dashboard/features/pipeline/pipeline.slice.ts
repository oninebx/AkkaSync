import { createEntityAdapter, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "@/store";
import { PipelineBase, PipelineDefinition, PipelineLive, PipelineOverview } from "./pipeline.types";

// export const pipelineAdapter = createEntityAdapter<Pipeline>();

// const pipelineSlice = createSlice({
//   name: 'pipeline',
//   initialState: pipelineAdapter.getInitialState(),
//   reducers: {
//     setPipelines: pipelineAdapter.setAll,
//     pipelineAdded: pipelineAdapter.addOne,
//     pipelineUpdated: pipelineAdapter.upsertOne,
//     pipelineRemoved: pipelineAdapter.removeOne
//   }
// });

// export const { setPipelines, pipelineAdded, pipelineUpdated, pipelineRemoved } = pipelineSlice.actions;
// export default pipelineSlice.reducer;
// export const pipelineSelectors = pipelineAdapter.getSelectors((state: RootState) => state.pipeline);

const pipelineAdapter = createEntityAdapter<PipelineBase>({
  sortComparer: (a, b) => a.name.localeCompare(b.name)
});

const initialState = pipelineAdapter.getInitialState({
  definition: {} as Record<string, PipelineDefinition>,
  overview: {} as Record<string, PipelineOverview>,
  live: {} as Record<string, PipelineLive>
});

const pipelineSlice = createSlice({
  name: 'pipeline',
  initialState,
  reducers: {
    setDefinitions: (state, action: PayloadAction<PipelineDefinition[]>) => {
      action.payload.forEach((d) => {
        state.definition[d.id] = d;

        pipelineAdapter.upsertOne(state, {
          id: d.id,
          name: d.name
        });
      });
    }
  } 
});

export { pipelineAdapter };
export const { setDefinitions } = pipelineSlice.actions;
export default pipelineSlice.reducer;