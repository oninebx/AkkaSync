import { createEntitySlice, EntityChangeHandler } from "@/store/createEntitySlice";
import { IPluginRuntmeExtraState, PluginInstance } from "./types";
import { ChangeOperation } from "@/infrastructure/realtime/types";
import { createSelector } from "@reduxjs/toolkit";

const handleRuntimeIndex: EntityChangeHandler<PluginInstance, IPluginRuntmeExtraState> = (
  state,
  data,
  operation
) => {
  switch(operation){
    case ChangeOperation.Upsert:
      data.forEach(item => {
        const { pipelineRunId, key, id } = item;
        if (!pipelineRunId || !key) return;

        if (!state.keyToIdIndex[pipelineRunId]) {
          state.keyToIdIndex[pipelineRunId] = {};
        }

        if (!state.keyToIdIndex[pipelineRunId][key]) {
          state.keyToIdIndex[pipelineRunId][key] = [];
        }

        const idList = state.keyToIdIndex[pipelineRunId][key];
        if (!idList.includes(id)) {
          idList.push(id);
        }
      });
      break;
    case ChangeOperation.Replace:
      const newIndex: Record<string, Record<string, string[]>> = {};
      
      data.forEach(item => {
        const { pipelineRunId, key, id } = item;
        if (!pipelineRunId || !key) return;

        if (!newIndex[pipelineRunId]) {
          newIndex[pipelineRunId] = {};
        }
        if (!newIndex[pipelineRunId][key]) {
          newIndex[pipelineRunId][key] = [];
        }
        
        newIndex[pipelineRunId][key].push(id);
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

const pluginRuntimeSelectors = {
  ...selectors,
  selectInstancesByRef: createSelector(
    [
      selectors.selectEntities,
      selectors.getExtraField('keyToIdIndex'),
      (_: any,__:string, pipelineRunId: string) => pipelineRunId,
      
    ],
    (entities, index, runId) => {

      const runIndex = index[runId];
      console.log(runId)
console.log(runIndex);
      if (!runIndex) return [];
      const allIds = Object.values(runIndex).flat();
      return allIds
        .map(id => entities[id])
        .filter((p): p is PluginInstance => !!p);
    }
  )
};
export {
  pluginRuntimeSelectors
}

export default slice.reducer;