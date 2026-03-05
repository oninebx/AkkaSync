import { RootState } from '@/store';
import { createSelector } from '@reduxjs/toolkit';
import { PluginListItem } from './plugin-hub.types';

const selectPluginSet = (state: RootState) => state.pluginHub.pluginSet;

export const selectPlugins = createSelector(
  [selectPluginSet],
  (pluginSet): PluginListItem[] => {
    const { entries, packages } = pluginSet;

    const packageMap = new Map(
      packages.map(p => [`${p.name}-${p.version}`, p])
    );

    return entries.map(entry => {
      const key = `${entry.name}-${entry.version}`;
      const matchedPackage = packageMap.get(key);

      return {
        id: `${entry.name}@${entry.version}`,
        type: 'source',
        name: entry.name,
        // version: entry.version,
        // status: entry.state,
        // url: matchedPackage?.downloadUrl ?? '',
        // actions: entry.state === 'Loaded' ? ['Unload', 'Update'] : ['Load', 'Remove']
      } as PluginListItem;
    });
  }
);