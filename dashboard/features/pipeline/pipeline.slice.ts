import { createEntityAdapter, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "@/store";
import { PipelineBase, PipelineDefinition, PipelineRun, PipelineOverview, PluginRun } from "./pipeline.types";

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

const runAdapter = createEntityAdapter<PipelineRun>({
  sortComparer: (a, b) => a.id.localeCompare(b.id)
});

// const initialState = pipelineAdapter.getInitialState({
//   definition: {} as Record<string, PipelineDefinition>,
//   overview: {} as Record<string, PipelineOverview>,
//   run: {} as Record<string, PipelineRun>
// });

const initialState = {
  base: pipelineAdapter.getInitialState({
    definition: {} as Record<string, PipelineDefinition>,
    overview: {} as Record<string, PipelineOverview>,
  }),
  
  run: runAdapter.getInitialState({
    byPipelinePlugin: {} as Record<string, Record<string, PluginRun[]>>,
  })
};

const pipelineSlice = createSlice({
  name: 'pipeline',
  initialState,
  reducers: {
    setDefinitions: (state, action: PayloadAction<PipelineDefinition[]>) => {
      action.payload.forEach((d) => {
        state.base.definition[d.id] = d;

        pipelineAdapter.upsertOne(state.base, {
          id: d.id,
          name: d.name
        });
      });
    },
    upsertRun: (state, action: PayloadAction<PipelineRun>) => {
      const run = action.payload;
      runAdapter.upsertOne(state.run, run);
      const pipelineId = run.id;
      const pluginMap = state.run.byPipelinePlugin[pipelineId] ?? {};
      Object.keys(pluginMap).forEach(k => delete pluginMap[k]);

      run.plugins && Object.values(run.plugins).forEach(pluginRun => {
        const key = pluginRun.key;
        const list = (pluginMap[key] ??= []);
        const exists = list.find(p => p.id === pluginRun.id);
        if (!exists) {
          list.push(pluginRun);
        }
        

      });



      state.run.byPipelinePlugin[pipelineId] = pluginMap;
    }    
  } 
});

export { pipelineAdapter, runAdapter };
export const { setDefinitions, upsertRun } = pipelineSlice.actions;
export default pipelineSlice.reducer;