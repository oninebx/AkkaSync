import { createEntitySlice } from "@/store/createEntitySlice";
import {  PluginRemote } from "./types";

const {selectors, slice} = createEntitySlice<PluginRemote>({
  name: 'pluginCentral',
  selectId: p => p.identifier
});

const pluginCentralSelectors = selectors;
export {
  pluginCentralSelectors
}

export default slice.reducer;