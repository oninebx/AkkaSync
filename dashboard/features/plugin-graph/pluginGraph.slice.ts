import { createEntityAdapter, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { PluginEdge, PluginInstance } from "./pluginGraph.type";

export const pluginInstanceAdapter = createEntityAdapter<PluginInstance>();
export const pluginEdgeAdapter = createEntityAdapter<PluginEdge>();

interface GraphState {
  instances: ReturnType<typeof pluginInstanceAdapter.getInitialState>;
  edges: ReturnType<typeof pluginEdgeAdapter.getInitialState>;
}

const initialState: GraphState = {
  instances: pluginInstanceAdapter.getInitialState(),
  edges: pluginEdgeAdapter.getInitialState()
};

const graphSlice = createSlice({
  name: 'pluginGraph',
  initialState,
  reducers: {
    setInstances: (state, action: PayloadAction<PluginInstance[]>) => {
      pluginInstanceAdapter.setAll(state.instances, action.payload);
    },
    pluginInstanceAdded: (state, action: PayloadAction<PluginInstance>) => {
      pluginInstanceAdapter.addOne(state.instances, action.payload);
    },
    pluginInstanceUpdated: (state, action: PayloadAction<PluginInstance>) => {
      pluginInstanceAdapter.upsertOne(state.instances, action.payload);
    },
    pluginInstanceRemoved: (state, action: PayloadAction<string>) => {
      pluginInstanceAdapter.removeOne(state.instances, action.payload);
    },
    setEdges: (state, action: PayloadAction<PluginEdge[]>) => {
      pluginEdgeAdapter.setAll(state.edges, action.payload);
    },
    pluginEdgeAdded: (state, action: PayloadAction<PluginEdge>) => {
      pluginEdgeAdapter.addOne(state.edges, action.payload);
    },
    pluginEdgeRemoved: (state, action: PayloadAction<string>) => {
      pluginEdgeAdapter.removeOne(state.edges, action.payload);
    }
  }
});

export const {
  setInstances,
  pluginInstanceAdded, 
  pluginInstanceUpdated,
  pluginInstanceRemoved,
  setEdges,
  pluginEdgeAdded,
  pluginEdgeRemoved
} = graphSlice.actions;

export const pluginGraphReducer = graphSlice.reducer;