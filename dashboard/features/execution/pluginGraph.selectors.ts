import { RootState } from "@/store";
import { pluginEdgeAdapter, pluginInstanceAdapter } from "./pluginGraph.slice";

const selectPluginInstanceState = (state: RootState) => state.pluginGraph.instances;
const selectPluginEdgeState = (state: RootState) => state.pluginGraph.edges;

const instanceSelectors = pluginInstanceAdapter.getSelectors(
  selectPluginInstanceState
);

const edgeSelectors = pluginEdgeAdapter.getSelectors(
  selectPluginEdgeState
);

export const selectPluginInstances = instanceSelectors.selectAll;
export const selectPluginEdges = edgeSelectors.selectAll;