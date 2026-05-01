import { ConnectorDefinition } from "./types";
import { createEntitySlice } from "@/store/createEntitySlice";

const {selectors, slice} = createEntitySlice<ConnectorDefinition>({
  name: 'connectorConfig',
  selectId: c => c.key
});

const connectorConfigSelectors = selectors;
export {
  connectorConfigSelectors
}

export default slice.reducer;