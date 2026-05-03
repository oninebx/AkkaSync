import pluginConfigReducer, { pluginConfigSelectors } from './pluginConfig.slice';
import pluginCacheReducer, { pluginCacheSelectors } from './pluginCache.slice';
import pluginCentralReducer, { pluginCentralSelectors } from './pluginCentral.slice';
import pluginRuntimeReducer, { pluginRuntimeSelectors } from './pluginRuntime.slice';

export {
  pluginConfigReducer,
  pluginCacheReducer,
  pluginCentralReducer,
  pluginRuntimeReducer,
  pluginConfigSelectors,
  pluginCacheSelectors,
  pluginCentralSelectors,
  pluginRuntimeSelectors
}