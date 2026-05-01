import { createEntitySlice } from "@/store/createEntitySlice";
import { PluginDefinition } from "./types";

const { slice, selectors } = createEntitySlice<PluginDefinition>({
  name: 'pluginConfig',
  selectId: p => p.identifier,
  sortComparer: (a, b) => a.key.localeCompare(b.key)
});

const pluginConfigSelectors = selectors;
export {
  pluginConfigSelectors
};

export default slice.reducer;