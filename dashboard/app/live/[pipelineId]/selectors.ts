// // export const selectPipelinePlugins = 

// import { selectPluginEdges, selectPluginInstances } from "@/features/execution/pluginGraph.selectors";
// import { PluginEdge, PluginInstance } from "@/features/execution/pluginGraph.type";
// import { createComplexFlow } from "@/mocks/plugin-graph";
import { RootState } from "@/store";
import { createSelector } from "@reduxjs/toolkit";
import { PluginAggregate, PluginNodeData, PluginNodeEdge, PluginNodePayload } from "./types";
import { PluginDefinition } from "@/features/pipeline/pipeline.types";
import { buildLayers, buildPluginGraph, layoutNodes } from "./utils";
// import { Edge, Node } from "@xyflow/react";

// // This function builds layers of plugin instances based on their dependencies defined by edges.
// function buildLayers(
//   instances: PluginInstance[], 
//   edges: PluginEdge[]
// ): PluginInstance[][] {

//   const instanceMap = new Map(instances.map(i => [i.id, i]));
//   const inDegree = new Map<string, number>();
//   const adjacencyList = new Map<string, string[]>();  

//   instances.forEach(i => {
//     inDegree.set(i.id, 0);
//     adjacencyList.set(i.id, []);
//   });

//   edges.forEach(e => {
//     inDegree.set(e.to, (inDegree.get(e.to) || 0) + 1);
//     adjacencyList.get(e.from)?.push(e.to);
//   });

//   const layers: PluginInstance[][] = [];
//   let currentLayer = instances.filter(i => (inDegree.get(i.id) || 0) === 0);
  
//   while (currentLayer.length > 0) {
//     layers.push(currentLayer);
//     const nextLayer: PluginInstance[] = [];
//     currentLayer.forEach(i => {
//       adjacencyList.get(i.id)?.forEach(toId => {
//         const toInstance = instanceMap.get(toId);
//         if (toInstance) {
//           inDegree.set(toId, (inDegree.get(toId) || 0) - 1);
//           if (inDegree.get(toId) === 0) {
//             nextLayer.push(toInstance);
//           }
//         }
//       });
//     });
//     currentLayer = nextLayer;
//   }

//   return layers;

// }

// function layoutNodes(layers: PluginInstance[][]): Node[]
// {
//   const layerGap = 250;
//   const nodeGap = 120;

//   const nodes: Node[] = [];
//   layers.forEach((layer, layerIndex) => {
//     const y = layerIndex * layerGap;
//     const totalWidth = (layer.length - 1) * nodeGap;
//     const startX = -totalWidth / 2;
//     layer.forEach((instance, nodeIndex) => {
//       const x = startX + nodeIndex * nodeGap;
//       nodes.push({
//         id: instance.id,
//         data: instance,
//         position: { x, y },
//         type: instance.type
//       });
//     });
//   });

//   return nodes;
// }

// // /**
// //  * Test Mock data
// //  */
// const USE_MOCK = true;
// const { instances, edges } = createComplexFlow();
// export const selectInstances = (state: any) => USE_MOCK ? instances : selectPluginInstances(state);
// export const selectEdges = (state: any) => USE_MOCK ? edges : selectPluginEdges(state);

// const selectFlowData = createSelector(
//   [selectInstances, selectEdges],
//   (
//     instances,
//     edges
//   ) => {
//     return { 
//       instances,
//       edges
//     };
//   }
// );

// const selectLayoutedFlowData = createSelector(
//   [selectFlowData],
//   (flowData):{nodes: Node[]; edges: Edge[]} => {
//     const { instances, edges } = flowData;
//     const layers = buildLayers(instances, edges);
//     const nodes = layoutNodes(layers);
//     const flowEdges: Edge[] = edges.map(e => ({
//       id: e.id,
//       source: e.from,
//       target: e.to,
//     }));
//     return {
//       nodes,
//       edges: flowEdges,
//     };
//   }
// );

// export { selectFlowData, selectLayoutedFlowData };



export const selectEffectivePlugins = createSelector(
  [
    (state: RootState, pipelineId: string) =>
      state.pipeline.definition[pipelineId]?.plugins ?? [],
    (state: RootState, pipelineId: string) =>
      state.pipeline.live[pipelineId]?.plugins ?? []
  ],
  (definitions, lives) => {
    return definitions.map((def, index) => {
      const live = lives[index];
      const { id: _, ...liveRest } = live ?? {};
      return {
        id: live?.id ?? def.key, // fallback id

        name: def.name,
        type: def.type,
        dependsOn: def.dependsOn ?? [],
        stats: { processed: 0, errors: 0 },
        status: 'idle',
        // ...DEFAULT_PLUGIN_LIVE,
        ...(liveRest ?? {})
      } as PluginAggregate;
    });
  }
);

export const selectPluginGraph = createSelector(
  [selectEffectivePlugins],
  (plugins) => {

    // 1. 构建 DAG graph
    const { nodes, edges } = buildPluginGraph(plugins);

    // 2. 拓扑分层
    const layers = buildLayers(nodes, edges);

    // 3. layout
    const layoutedNodes = layoutNodes(layers);
    console.log('edges', edges);
    return {
      nodes: layoutedNodes,
      edges
    };
  }
);

// export const selectPluginGraph = createSelector(
//   [selectEffectivePlugins],
//   (plugins) => {
//     const nodes = plugins.map((p: any, index: number) => ({
//       id: p.id,
//       data: p,
//       position: { x: index * 250, y: 100 }
//     }));

//     const edges = plugins.slice(1).map((p: any, i: number) => ({
//       id: `${plugins[i].id}-${p.id}`,
//       source: plugins[i].id,
//       target: p.id
//     }));

//     return { nodes, edges };
//   }
// );