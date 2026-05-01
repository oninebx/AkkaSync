import { createEntitySlice } from "@/store/createEntitySlice";
import { PipelineMetrics } from "./types";

const {slice, actions, selectors, adapter} = createEntitySlice<PipelineMetrics>({
  name: 'pipelineRuntime',
  selectId: p => p.identifier,
  sortComparer: (a, b) => a.name.localeCompare(b.name)
});

const pipelineRuntimeSelectors = selectors;

export {
  pipelineRuntimeSelectors
};

export default slice.reducer;