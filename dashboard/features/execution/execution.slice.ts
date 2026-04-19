import { createEntityAdapter, createSlice, PayloadAction } from "@reduxjs/toolkit";

const pluginInstanceAdapter = createEntityAdapter<PluginInstance>({
  sortComparer: false
});

const pluginLiveAdapter = createEntityAdapter<PluginLive>({
  sortComparer: false
});

const initialState = {
  instances: pluginInstanceAdapter.getInitialState(),
  lives: pluginLiveAdapter.getInitialState()
};

const executionSlice = createSlice({
  name: 'execution',
  initialState,
  reducers: {
    setPluginInstances(state, action: PayloadAction<PluginInstance[]>) {
      pluginInstanceAdapter.setAll(state.instances, action.payload)
    },
    upsertPluginInstances(state, action: PayloadAction<PluginInstance[]>) {
      pluginInstanceAdapter.upsertMany(state.instances, action.payload)
    },

    setPluginLives(state, action: PayloadAction<PluginLive[]>) {
      pluginLiveAdapter.setAll(state.lives, action.payload)
    },
    upsertPluginLives(state, action: PayloadAction<PluginLive[]>) {
      pluginLiveAdapter.upsertMany(state.lives, action.payload)
    },
    patchPluginLive(
      state,
      action: PayloadAction<{
        id: string
        processedDelta?: number
        errorDelta?: number
      }>
    ) {
      const { id, processedDelta, errorDelta } = action.payload
      const existing = state.lives.entities[id]

      if (!existing) {
        pluginLiveAdapter.addOne(state.lives, {
          id,
          processed: processedDelta ?? 0,
          errors: errorDelta ?? 0,
          // updatedAt: Date.now(),
        })
        return
      }

      existing.processed =
        (existing.processed ?? 0) + (processedDelta ?? 0)

      existing.errors =
        (existing.errors ?? 0) + (errorDelta ?? 0)


      // existing.updatedAt = Date.now()
    },
  }
});

export { pluginInstanceAdapter, pluginLiveAdapter };
export const { setPluginInstances, upsertPluginInstances, setPluginLives, upsertPluginLives, patchPluginLive } = executionSlice.actions;
export const executionReducer = executionSlice.reducer;