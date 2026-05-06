import { createEntitySlice, EntityChangeHandler } from "@/store/createEntitySlice";
import { IPluginRuntmeExtraState, PluginInstance } from "./types";
import { ChangeOperation } from "@/infrastructure/realtime/types";



const handleRuntimeIndex: EntityChangeHandler<PluginInstance, IPluginRuntmeExtraState> = (
  state,
  data,
  operation
) => {
  switch(operation){
    case ChangeOperation.Upsert:
      data.forEach(item => {
        if(item.key){
          state.keyToIdIndex[item.key] = item.id;
        }
      });
      break;
    case ChangeOperation.Replace:
      const newIndex: Record<string, string> = {};
      data.forEach(item => {
        if (item.key) {
          newIndex[item.key] = item.id;
        }
      });
      state.keyToIdIndex = newIndex;
      break;
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