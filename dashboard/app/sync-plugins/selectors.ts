import { createSelector } from '@reduxjs/toolkit';
import { PluginHealthStatus, PluginVM } from './types';
import { pluginCacheSelectors, pluginCentralSelectors } from '@/features/plugins';
import { pipelineConfigSelectors } from '@/features/pipelines/pipelineConfig.slice';
import { PluginKind } from '@/contracts/plugin/types';

export const selectPluginPackages = createSelector(
  [
    pluginCacheSelectors.selectEntities,
    pluginCentralSelectors.selectEntities,
    pipelineConfigSelectors.selectEntities
  ],
  (cache, central, pipelines): PluginVM[] => {

    const usageCountMap: Record<string, number> = {};
    const pipelineKindMap = new Map<string, PluginKind>();

    Object.values(pipelines).forEach((pipe) => {
      if (!pipe?.plugins) return;
      Object.values(pipe.plugins).forEach((info: any) => {
        const { provider, kind } = info;
        if (provider) {
          usageCountMap[provider] = (usageCountMap[provider] || 0) + 1;
          if (kind) pipelineKindMap.set(provider, kind);
        }
      });
    });

    const allProviderIds = new Set([
      ...Object.keys(cache),
      ...Object.keys(central),
      ...Object.keys(usageCountMap)
    ]);

    return Array.from(allProviderIds).map((provider) => {
      const local = cache[provider];
      const remote = central[provider];
      
      const isUsed = !!usageCountMap[provider];
      const isInstalled = !!local;
      const isRemoteAvailable = !!remote;

      let status: PluginHealthStatus = 'loaded';

      if (!isInstalled) {
        status = isRemoteAvailable ? 'updateAvailable' : 'notFound';
      } else {
        // 已安装：检查是否有加载错误，再检查版本更新
       /* if (local.hasLoadError) {
          status = 'loadFailed';
        } else */if (isRemoteAvailable && remote.version !== local.version) {
          status = 'updateAvailable';
        } else {
          status = 'loaded';
        }
      }

      // 提取 Kind 逻辑：本地 > Pipeline引用 > 远程 > 默认
      // const kind = local?.kind 
      //   ? extractKind(local.kind) 
      //   : (pipelineKindMap.get(provider) || (remote?.kind as PluginKind) || 'transform');
      const kind = (local?.kind || pipelineKindMap.get(provider) || 'transform') as PluginKind

      return {
        id: provider,
        name: provider,
        kind: kind,
        latestversion: remote?.version,
        installedVersion: local?.version,
        usedByCount: usageCountMap[provider] ?? 0,
        status: status,
        actions: status === 'updateAvailable' ? 'update' : 'manage'
      };
    });
  }
);