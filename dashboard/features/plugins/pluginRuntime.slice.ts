import { createEntitySlice, EntityChangeHandler } from "@/store/createEntitySlice";
import { PluginInstance } from "./types";
import { ChangeOperation } from "@/infrastructure/realtime/types";

interface IPluginRuntmeExtraState {
  keyToIdIndex: Record<string, string>;
}

const handleRuntimeIndex: EntityChangeHandler<PluginInstance, IPluginRuntmeExtraState> = (
  state,
  data,
  operation
) => {
  if(operation === ChangeOperation.Upsert){
    data.forEach(item => {
      if(item.key){
        state.keyToIdIndex[item.key] = item.id;
      }
    });
  }
}

const { slice, selectors } = createEntitySlice<PluginInstance, string, IPluginRuntmeExtraState>({
  name: 'pluginRuntime',
  selectId: p => p.identifier,
  additionalInitialState: {
    keyToIdIndex: {}
  },
  onChanges: handleRuntimeIndex
});

const pluginRuntimeSelectors = selectors;
export {
  pluginRuntimeSelectors
}

export default slice.reducer;