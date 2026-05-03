import { connectorConfigSelectors } from "@/features/connector";
import { pipelineConfigSelectors } from "@/features/pipelines";
import { pluginCacheSelectors, pluginCentralSelectors, pluginConfigSelectors } from "@/features/plugins";
import { createSelector } from "@reduxjs/toolkit";
import { ConnectorNodeData, FlowEdge, FlowNodeData } from "./types";
import { PluginHealthStatus } from "@/contracts/plugin/types";
import { getPluginHealthStatus } from "@/features/plugins/utils";
import { getEffectivePosition, NodeLayout } from "@/infrastructure/storage/liveLayout";

const selectPipelines = pipelineConfigSelectors.selectEntities;
const selectConnectors = connectorConfigSelectors.selectEntities;
const selectPlugins = pluginConfigSelectors.selectAll;
const selectCachePlugins = pluginCacheSelectors.selectEntities;
const selectCentralPlugins = pluginCentralSelectors.selectEntities;

const selectPipelineTopology = createSelector(
  [
    selectPipelines,
    selectConnectors,
    selectPlugins,
    selectCachePlugins,
    selectCentralPlugins,
    (_, pipelineId: string) => pipelineId,
    (_, __, savedLayout: NodeLayout | null) => savedLayout
  ],
  (pipelines, connectors, configPlugins, cachePlugins, centralPlugins, id, savedLayout) => {
    const nodes: FlowNodeData[] = [];
    const edges: FlowEdge[] = [];
    
    const pipeline = pipelines[id];
    if (!pipeline) return { nodes, edges };

    // 1. Process Source Connector
    const sourceConnector = connectors[pipeline.sourceId];
    const sourceConnectorId = sourceConnector ? `source-${sourceConnector.key}` : null;
    
    if (sourceConnector) {
      nodes.push({
        id: sourceConnectorId!,
        type: 'connector',
        position: getEffectivePosition(sourceConnectorId!, { x: 0, y: 200 }, savedLayout),
        data: {
          id: sourceConnector.key,
          name: sourceConnector.name,
          role: 'SOURCE'
        }
      } as ConnectorNodeData);
    }

    // 2. Process Sink Connectors
    const pluginToSinkId: Record<string, string> = {};
    const sinkConnectorNodes = (pipeline.sinkIds || [])
      .map(sinkId => connectors[sinkId])
      .filter(Boolean)
      .map((connector, idx) => {
        const sinkConnectorId = `sink-${connector!.key}`;
        pluginToSinkId[connector.plugin] = sinkConnectorId;
        return {
        id: sinkConnectorId,
        type: 'connector',
        position: getEffectivePosition(sinkConnectorId, { x: 1200, y: idx * 250 + 100 }, savedLayout),
        data: {
          id: connector!.key,
          name: connector!.name,
          role: 'SINK'
        }
      } as ConnectorNodeData});
    
    nodes.push(...sinkConnectorNodes);

    // 3. Process Pipeline Plugins
    const pipelinePlugins = configPlugins.filter(p => p.pipeline === id);

    pipelinePlugins.forEach((config, index) => {
      const provider = config.provider;
      const central = centralPlugins[provider];
      const cache = cachePlugins[provider];
      const health = getPluginHealthStatus(cache, central);
      const isAnimated = health === 'loaded' || health === 'updateAvailable';
      const nodeId = config.identifier;
      nodes.push({
        id: nodeId,
        type: 'plugin',
        position: getEffectivePosition(nodeId, { x: 300 + index * 280, y: 200 }, savedLayout),
        data: {
          id: nodeId,
          name: provider,
          provider: provider,
          kind: config.type || 'transform',
          health: health,
          version: cache?.version,
          latestVersion: central?.version,
        }
      });

      // 4. Establish Edges

      // Connection: Source Connector -> Source Plugin
      if(sourceConnector.plugin === nodeId){
        edges.push({
          id: `e-src-ingest-${nodeId}`,
          source: sourceConnectorId!,
          target: nodeId,
          animated: isAnimated,
          style: { stroke: '#3b82f6', strokeWidth: 3 }
        });
      }

      // Connection: Sink Plugin -> Sink Connector
      const sinkConnectorId = pluginToSinkId[nodeId];
      if(sinkConnectorId) {
        edges.push({
            id: `e-sink-deliver-${nodeId}-${sinkConnectorId}`,
            source: nodeId,
            target: sinkConnectorId,
            animated: isAnimated, // Animate only if healthy
            style: { stroke: '#10b981', strokeWidth: 3 },
          });
      }

      // Connection: Plugin Dependencies (DAG)
      (config.dependsOn || []).forEach((parentKey) => {
        const parent = pipelinePlugins.find((p) => p.key === parentKey);
        if (parent) {
          edges.push({
            id: `e-${parent.identifier}-${nodeId}`,
            source: parent.identifier,
            target: nodeId,
            animated: isAnimated,
          });
        }
      });
    });

    return { nodes, edges };
  }
);

export {
  selectPipelineTopology
}