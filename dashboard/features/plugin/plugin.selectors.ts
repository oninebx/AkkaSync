import { RootState } from '@/store';
import { pluginAdapter } from './plugin.slice';

export const pluginSelectors = pluginAdapter.getSelectors(
  (state: RootState) => state.plugin
);

