import { createEntitySlice } from "@/store/createEntitySlice";
import { PipelineDefinition } from "./types";

const {slice, actions, selectors, adapter} = createEntitySlice<PipelineDefinition>({
  name: 'pipelineConfig',
  selectId: p => p.identifier,
  sortComparer: (a, b) => a.name.localeCompare(b.name)
});

const pipelineConfigSelectors = selectors;

export {
  pipelineConfigSelectors
};

export default slice.reducer;