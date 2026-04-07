import { pluginSelectors } from "@/features/plugin-artifact/plugin.selectors";
import { PluginVM } from "./types";
import { RootState } from "@/store";
import { createSelector } from "@reduxjs/toolkit";

export const selectPluginData = createSelector(
  [
    pluginSelectors.selectAll,
    (state: RootState) => state.plugin.packages
  ],
  (plugins, packages): PluginVM[] => {
    const map = new Map<string, PluginVM>();

    for (const pkg of packages) {
      const key = pkg.id;
      const parts = pkg.id.split('.');
      const type = parts[2];
      const name = parts[3];

      map.set(key, {
        id: pkg.id,
        type,
        name,
        latestversion: pkg.version,
        url: pkg.downloadUrl ?? '',
        pkgChecksum: pkg.checksum.replace(/^sha256:/i, '') ?? ''
      } as PluginVM);
    }

    for (const plugin of plugins) {
      const key = plugin.id;
      const parts = plugin.id.split('.');
      const type = parts[2];
      const name = parts[3];

      const existing = map.get(key);

      if (existing) {
        map.set(key, {
          ...existing,
          installedVersion: plugin.version
        });
      } else {
        
        map.set(key, {
          id: plugin.id,
          type,
          name,
          installedVersion: plugin.version,
        } as PluginVM);
      }
    }
    console.log(map)
    
    return Array.from(map.values());
  }
);