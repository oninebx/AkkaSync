import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { pluginActions } from "./plugin.slice";
import { PluginEntry, PluginPackageEntry, PluginSet } from "./plugin.types";

envelopeHandlerMap.set('plugin.entries.initialized',  (event, dispatch) => {
  dispatch(pluginActions.setPlugins(event.payload as PluginSet))
});

envelopeHandlerMap.set('plugin.entries.added', (event, dispatch) => {
  dispatch(pluginActions.loadPlugin(event.payload as PluginEntry));
});

envelopeHandlerMap.set('plugin.entries.removed', (event, dispatch) => {
  dispatch(pluginActions.unloadPlugin(String(event.payload)));
});

envelopeHandlerMap.set('plugin.packages.checked', (event, dispatch) => {
  dispatch(pluginActions.notifyPackages(event.payload as PluginPackageEntry[]))
});

