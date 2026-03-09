import { RootState } from '@/store';
import { createSelector } from '@reduxjs/toolkit';
import { PluginListItem } from './plugin-hub.types';

const selectPluginSet = (state: RootState) => state.pluginHub.pluginSet;

export const selectPlugins = createSelector(
  [selectPluginSet],
  (pluginSet): PluginListItem[] => {
    const { entries, packages } = pluginSet;
    const map = new Map<string, PluginListItem>();

    for (const p of packages) {
      const key = `${p.id}-${p.version}`;

      map.set(key, {
        id: `${p.id}@${p.version}`,
        type: 'source',
        name: p.id,
        version: p.version,
        // url: p.downloadUrl ?? ''
      } as PluginListItem);
    }
    console.log(map)

    for (const e of entries) {
      const key = `${e.name}-${e.version}`;
      const existing = map.get(key);

      if (existing) {
        map.set(key, {
          ...existing,
          // status: e.state
        });
      } else {
        // entries 独有
        map.set(key, {
          id: `${e.name}@${e.version}`,
          type: 'source',
          name: e.name,
          version: e.version,
          // status: e.state
        } as PluginListItem);
      }
    }

    return Array.from(map.values());
  }
);