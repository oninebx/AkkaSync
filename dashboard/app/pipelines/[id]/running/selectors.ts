import { createSelector } from "@reduxjs/toolkit";
import { connectorConfigSelectors } from "@/features/connector";
import { pipelineConfigSelectors } from "@/features/pipelines";
import { pluginConfigSelectors, pluginRuntimeSelectors } from "@/features/plugins";
import { getEffectivePosition, NodeLayout } from "@/infrastructure/storage/liveLayout";
import { 
  FlowNodeData, 
  FlowEdge, 
  PluginRuntimeNodeData, 
  ConnectorNodeData 
} from "../types";

const selectPipelines = pipelineConfigSelectors.selectEntities;
const selectConnectors = connectorConfigSelectors.selectEntities;
const selectPlugins = pluginConfigSelectors.selectAll;
const selectInstancePlugins = pluginRuntimeSelectors.selectAll;

export const selectRuntimeTopology = createSelector(
  [
    selectPipelines,
    selectConnectors,
    selectPlugins,
    selectInstancePlugins,
    (_, pipelineId: string) => pipelineId,
    (_, __, savedLayout: NodeLayout | null) => savedLayout
  ],
  (pipelines, connectors, configPlugins, allInstances, id, savedLayout) => {
    const nodes: FlowNodeData[] = [];
    const edges: FlowEdge[] = [];
    
    const pipeline = pipelines[id];

    if (!pipeline) return { nodes, edges };

    // Map config plugins by key for quick lookup during instance iteration
    const configMap = new Map(configPlugins.map(p => [p.key, p]));

    // --- 1. Process Connector Nodes (Sources & Sinks) ---
    
    // Handle Source Connector
    const sourceConnector = connectors[pipeline.sourceId];
    const sourceNodeId = sourceConnector ? `source-${sourceConnector.key}` : null;
    
    if (sourceConnector) {
      nodes.push({
        id: sourceNodeId!,
        type: 'connector',
        position: getEffectivePosition(sourceNodeId!, { x: 0, y: 300 }, savedLayout),
        data: { id: sourceConnector.key, name: sourceConnector.name, role: 'SOURCE' }
      } as ConnectorNodeData);
    }

    // Handle Sink Connectors
    const sinkInfos = (pipeline.sinkIds || [])
      .map(sid => connectors[sid])
      .filter(Boolean)
      .map((conn, idx) => {
        const sid = `sink-${conn!.key}`;
        nodes.push({
          id: sid,
          type: 'connector',
          position: getEffectivePosition(sid, { x: 1400, y: idx * 250 + 150 }, savedLayout),
          data: { id: conn!.key, name: conn!.name, role: 'SINK' }
        } as ConnectorNodeData);
        
        // Return metadata for edge creation: associated Plugin ID and the Node ID in the graph
        return { pluginId: conn!.plugin, nodeId: sid };
      });

    // --- 2. Process Runtime Plugin Instance Nodes ---

    // Filter and process instances belonging to the current pipeline
    // (Assumes backend-returned instances are linked to config via 'key')
    allInstances.forEach((inst, index) => {
      const config = configMap.get(inst.key);
      if (!config) return;

      const nodeId = inst.identifier; // Use unique identifier as React Flow Node ID
      // const isActive = inst.processed > 0;
      const isActive = inst.usedBy > 0;

      nodes.push({
        id: nodeId,
        type: 'runtimePlugin', // Custom runtime component type
        position: getEffectivePosition(nodeId, { x: 500, y: 100 + index * 180 }, savedLayout),
        data: {
          id: nodeId,
          name: config.key || config.provider,
          kind: config.type || 'transform',
          processed: inst.processed,
          errors: inst.errors,
          // isRunning: isActive
        }
      } as PluginRuntimeNodeData);

      // --- 3. Establish Edges (Based on instances and dependency config) ---

      // A. Connect Source Connector -> Corresponding Plugin Instance
      if (sourceConnector && sourceConnector.plugin === config.identifier) {
        edges.push({
          id: `e-src-${nodeId}`,
          source: sourceNodeId!,
          target: nodeId,
          animated: isActive,
          style: { stroke: '#3b82f6', strokeWidth: 2 }
        });
      }

      // B. Connect dependencies between plugin instances (DAG Logic)
      // If the current plugin depends on a parent, find all runtime instances of that parent
      (config.dependsOn || []).forEach(parentKey => {
        const parentInstances = allInstances.filter(i => i.key === parentKey);
        parentInstances.forEach(pInst => {
          edges.push({
            id: `e-${pInst.identifier}-${nodeId}`,
            source: pInst.identifier,
            target: nodeId,
            animated: isActive
          });
        });
      });

      // C. Connect Plugin Instance -> Sink Connector
      sinkInfos.forEach(sink => {
        if (sink.pluginId === config.identifier) {
          edges.push({
            id: `e-sink-${nodeId}-${sink.nodeId}`,
            source: nodeId,
            target: sink.nodeId,
            animated: isActive,
            style: { stroke: '#10b981', strokeWidth: 2 }
          });
        }
      });
    });
    
    return { nodes, edges };
  }
);