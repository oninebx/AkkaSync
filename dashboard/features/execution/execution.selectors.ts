import { RootState } from "@/store";
import { pluginNodeAdapter } from "./execution.slice";

export const pluginNodeSelectors = pluginNodeAdapter.getSelectors(
  (state: RootState) => state.execution
);

export const selectPluginNodes = pluginNodeSelectors.selectAll;
export const selectPluginNodeEntities = pluginNodeSelectors.selectEntities;
export const selectPluginNodeIds = pluginNodeSelectors.selectIds;
export const selectPluginNodeById = pluginNodeSelectors.selectById;