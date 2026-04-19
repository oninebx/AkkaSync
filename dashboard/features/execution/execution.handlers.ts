import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { setPluginInstances } from "./execution.slice";

envelopeHandlerMap.set('sync.plugins.added', (event, dispatch) => 
  dispatch(setPluginInstances(event.payload as PluginInstance[])));