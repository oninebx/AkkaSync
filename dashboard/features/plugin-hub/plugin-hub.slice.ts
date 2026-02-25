import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { PluginEntry, PluginSet } from "./plugin-hub.types";

interface PluginHubState {
  pluginSet: PluginSet
}

const initialState: PluginHubState = {
  pluginSet: { entries: [] }
}

export const pluginHubSlice = createSlice({
  name: 'plugin-hub',
  initialState,
  reducers: {
    setPlugins(state, action: PayloadAction<PluginSet>) {
      state.pluginSet = action.payload;
    },
    loadPlugin(state, action: PayloadAction<PluginEntry>) {
      const entry = state.pluginSet.entries.find(e => e.name === action.payload.name);
      if(entry){
        entry.version = action.payload.version;
        entry.state = 'Loaded';
      }
    },
    unloadPlugin(state, action: PayloadAction<string>){
      const entry = state.pluginSet.entries.find(e => e.name === action.payload);
      console.log(entry, action.payload);
      if(entry){
        entry.version = '-';
        entry.state = 'Unloaded';
        console.log(entry)
      }
    }
  }
});

export const pluginHubActions = pluginHubSlice.actions;
export const pluginHubReducers = pluginHubSlice.reducer;

