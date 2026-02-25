import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { pluginHubActions } from "./plugin-hub.slice";
import { PluginEntry, PluginSet } from "./plugin-hub.types";

envelopeHandlerMap.set('pluginhub.entries.initialized',  (event, dispatch) => {
  dispatch(pluginHubActions.setPlugins(event.payload as PluginSet))
});

envelopeHandlerMap.set('pluginhub.entries.added', (event, dispatch) => {
  dispatch(pluginHubActions.loadPlugin(event.payload as PluginEntry));
});

envelopeHandlerMap.set('pluginhub.entries.removed', (event, dispatch) => {
  dispatch(pluginHubActions.unloadPlugin(event.payload as PluginEntry));
});

