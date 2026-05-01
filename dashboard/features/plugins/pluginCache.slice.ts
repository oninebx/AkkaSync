import { createEntitySlice } from "@/store/createEntitySlice";
import { PluginLocal } from "./types";

const {selectors, slice} = createEntitySlice<PluginLocal>({
  name: 'pluginCache',
  selectId: p => p.identifier
});

const pluginCacheSelectors = selectors;
export {
  pluginCacheSelectors
}

export default slice.reducer;