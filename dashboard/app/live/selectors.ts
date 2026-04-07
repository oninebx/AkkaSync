import { selectPluginEdges, selectPluginInstances } from "@/features/plugin-graph/pluginGraph.selectors";
import { PluginEdge, PluginInstance } from "@/features/plugin-graph/pluginGraph.type";
import { createComplexFlow } from "@/mocks/plugin-graph";
import { createSelector } from "@reduxjs/toolkit";
import { Edge, Node } from "@xyflow/react";

// This function builds layers of plugin instances based on their dependencies defined by edges.
function buildLayers(
  instances: PluginInstance[], 
  edges: PluginEdge[]
): PluginInstance[][] {

  const instanceMap = new Map(instances.map(i => [i.id, i]));
  const inDegree = new Map<string, number>();
  const adjacencyList = new Map<string, string[]>();  

  instances.forEach(i => {
    inDegree.set(i.id, 0);
    adjacencyList.set(i.id, []);
  });

  edges.forEach(e => {
    inDegree.set(e.to, (inDegree.get(e.to) || 0) + 1);
    adjacencyList.get(e.from)?.push(e.to);
  });

  const layers: PluginInstance[][] = [];
  let currentLayer = instances.filter(i => (inDegree.get(i.id) || 0) === 0);
  
  while (currentLayer.length > 0) {
    layers.push(currentLayer);
    const nextLayer: PluginInstance[] = [];
    currentLayer.forEach(i => {
      adjacencyList.get(i.id)?.forEach(toId => {
        const toInstance = instanceMap.get(toId);
        if (toInstance) {
          inDegree.set(toId, (inDegree.get(toId) || 0) - 1);
          if (inDegree.get(toId) === 0) {
            nextLayer.push(toInstance);
          }
        }
      });
    });
    currentLayer = nextLayer;
  }

  return layers;

}

function layoutNodes(layers: PluginInstance[][]): Node[]
{
  const layerGap = 250;
  const nodeGap = 120;

  const nodes: Node[] = [];
  layers.forEach((layer, layerIndex) => {
    const y = layerIndex * layerGap;
    const totalWidth = (layer.length - 1) * nodeGap;
    const startX = -totalWidth / 2;
    layer.forEach((instance, nodeIndex) => {
      const x = startX + nodeIndex * nodeGap;
      nodes.push({
        id: instance.id,
        data: instance,
        position: { x, y },
        type: instance.type
      });
    });
  });

  return nodes;
}

/**
 * Test Mock data
 */
const USE_MOCK = true;
const { instances, edges } = createComplexFlow();
export const selectInstances = (state: any) => USE_MOCK ? instances : selectPluginInstances(state);
export const selectEdges = (state: any) => USE_MOCK ? edges : selectPluginEdges(state);

const selectFlowData = createSelector(
  [selectInstances, selectEdges],
  (
    instances,
    edges
  ) => {
    return { 
      instances,
      edges
    };
  }
);

const selectLayoutedFlowData = createSelector(
  [selectFlowData],
  (flowData):{nodes: Node[]; edges: Edge[]} => {
    const { instances, edges } = flowData;
    const layers = buildLayers(instances, edges);
    const nodes = layoutNodes(layers);
    const flowEdges: Edge[] = edges.map(e => ({
      id: e.id,
      source: e.from,
      target: e.to,
    }));
    return {
      nodes,
      edges: flowEdges,
    };
  }
);

export { selectFlowData, selectLayoutedFlowData };