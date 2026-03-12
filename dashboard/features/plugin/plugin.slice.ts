import { createEntityAdapter, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { PluginEntry, PluginPackageEntry, PluginSet } from "./plugin.types";

export const pluginAdapter = createEntityAdapter<PluginEntry>();


type PluginState = ReturnType<typeof pluginAdapter.getInitialState> & {
  packages: PluginPackageEntry[];
}

const initialState: PluginState = pluginAdapter.getInitialState({
  packages: []
});

export const pluginHubSlice = createSlice({
  name: 'plugin',
  initialState,
  reducers: {
    setPlugins(state, action: PayloadAction<PluginSet>) {
      pluginAdapter.setAll(state, action.payload.entries);
      state.packages = action.payload.packages ?? [];
    },
    loadPlugin(state, action: PayloadAction<PluginEntry>) {
      pluginAdapter.upsertOne(state, { ...action.payload, status: 'loaded' });
    },
    unloadPlugin(state, action: PayloadAction<string>){
      const name = action.payload;

      pluginAdapter.updateOne(state, {
        id: name,
        changes: {  version: '-', status: 'unloaded' }
      });
    },
    notifyPackages(state, action: PayloadAction<PluginPackageEntry[]>) {
      state.packages = action.payload;
    }
  }
});

export const pluginActions = pluginHubSlice.actions;
export const pluginReducers = pluginHubSlice.reducer;

