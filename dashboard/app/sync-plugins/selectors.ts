// import { pluginSelectors } from "@/features/plugin-artifact/plugin.selectors";
// import { PluginVM } from "./types";
// import { RootState } from "@/store";
// import { createSelector } from "@reduxjs/toolkit";

// export const selectPluginData = createSelector(
//   [
//     pluginSelectors.selectAll,
//     (state: RootState) => state.plugin.packages
//   ],
//   (plugins, packages): PluginVM[] => {
//     const map = new Map<string, PluginVM>();

//     for (const pkg of packages) {
//       const key = pkg.id;
//       const parts = pkg.id.split('.');
//       const type = parts[2];
//       const name = parts[3];

//       map.set(key, {
//         id: pkg.id,
//         type,
//         name,
//         latestversion: pkg.version,
//       } as PluginVM);
//     }

//     for (const plugin of plugins) {
//       const key = plugin.id;
//       const parts = plugin.id.split('.');
//       const type = parts[2];
//       const name = parts[3];

//       const existing = map.get(key);

//       if (existing) {
//         map.set(key, {
//           ...existing,
//           installedVersion: plugin.version
//         });
//       } else {

//         map.set(key, {
//           id: plugin.id,
//           type,
//           name,
//           installedVersion: plugin.version,
//         } as PluginVM);
//       }
//     }
//     console.log(map)

//     return Array.from(map.values());
//   }
// );

import { createSelector } from '@reduxjs/toolkit';
import { PluginHealthStatus, PluginVM } from './types';
import { pluginCacheSelectors, pluginCentralSelectors } from '@/features/plugins';
import { pipelineConfigSelectors } from '@/features/pipelines/pipelineConfig.slice';
import { PluginKind } from '@/contracts/plugin/types';
import { PluginLocal, PluginRemote } from '@/features/plugins/types';
import { PluginInfo } from '@/features/pipelines/types';

export const selectPluginPackages = createSelector(
  [
    pluginCacheSelectors.selectEntities,
    pluginCentralSelectors.selectEntities,
    pipelineConfigSelectors.selectEntities],
  (cache, central, pipelines): PluginVM[] => {

    const usageCountMap: Record<string, number> = {};
    const referencedProviders = new Map<string, PluginKind>();

    Object.values(pipelines).forEach((pipe: any) => {
      if (!pipe?.plugins) return;

      Object.values(pipe.plugins).forEach((info: any) => {
        const { provider, kind } = info as PluginInfo;
        if (typeof provider === 'string') {
          usageCountMap[provider] = (usageCountMap[provider] || 0) + 1;
          referencedProviders.set(provider, kind);
        }
      });
    });

    // const allProviders = new Set([
    //   ...Object.keys(cache),
    //   ...Array.from(referencedProviders)
    // ]);

    return Array.from(referencedProviders.keys()).map(provider => {
      const local = cache[provider];

      const remote = local ? central[local.name] : undefined;

      // let kind: PluginKind = 'transform';
      // if (local) {
      //   kind = extractKind(local.kind);
      // }
      let status: PluginHealthStatus = 'loaded';

      // 1. load failed 优先
      // if (local?.hasLoadError) {
      //   status = 'loadFailed';
      // }

      // 2. 本地不存在
      if (!local) {
        // 如果远程存在 => updateAvailable（其实是“可安装”）
        if (remote) {
          status = 'updateAvailable';
        } else if (referencedProviders.has(provider)) {
          status = 'notFound';
        }
        else {
          status = 'notFound';
        }
      }

      else {
        if (remote && remote.version !== local.version) {
          status = 'updateAvailable';
        } else {
          status = 'loaded';
        }
      }
      console.log(local, remote);
      console.log(provider, status);
      return {
        id: local?.identifier ?? `missing-${provider}`,
        name: provider,
        kind: referencedProviders.get(provider) ?? 'unknown',
        latestversion: remote?.version,
        installedVersion: local?.version,
        usedByCount: usageCountMap[provider] ?? 0,
        status: status,
        actions: status === 'updateAvailable' ? 'update' : 'manage'
      };
    });

    // const cacheById = Object.values(cache).reduce((acc, local) => {

    //   if (local) acc[local.name] = local;
    //   return acc;
    // }, {} as Record<string, PluginLocal>);

    // const centralById = Object.values(central).reduce((acc, remote) => {
    //   if (remote) acc[remote.identifier] = remote;
    //   return acc;
    // }, {} as Record<string, PluginRemote>);

    // const allIdentifiers = Array.from(new Set([
    //   ...Object.keys(cacheById),
    //   ...Object.keys(centralById)
    // ]));

    // return allIdentifiers.map(id => {
    //   const local = cacheById[id];
    //   const remote = centralById[id];

    //   const kind = local ? extractKind(local.kind) : 'source';

    //   let status: PluginHealthStatus = 'loadFailed';
    //   if (local && remote) {
    //     status = remote.version !== local.version ? 'updateAvailable' : 'loaded';
    //   } else if (local) {
    //     status = 'loaded';
    //   }

    //   return {
    //     id: id,
    //     name: local?.provider ?? remote?.qualifiedName ?? 'Unknown',
    //     kind: kind,
    //     latestversion: remote?.version,
    //     installedVersion: local?.version,
    //     usedByCount: 0,
    //     status: status,
    //     actions: status === 'updateAvailable' ? 'update' : 'manage'
    //   };
    // });

  }
);

const extractKind = (rawKind: string): PluginKind => {

  const kind = rawKind.replace(/^ISync/, '').toLowerCase();
  return kind as PluginKind;

  // // 检查是否符合预期，否则返回默认值或抛错
  // const validKinds: PluginKind[] = ['source', 'transform', 'sink'];
  // return validKinds.includes(kind as PluginKind) ? (kind as PluginKind) : 'source';

};