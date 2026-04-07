// src/mocks/plugin-graph/scenarios/complexFlow.ts
import { createPluginInstance, createPluginEdge } from "../generators";

export function createComplexFlow() {
  const pipelineId = "pipeline-002";

  const instances = [
    createPluginInstance('s1', 'source', pipelineId),
    createPluginInstance('s2', 'source', pipelineId),
    createPluginInstance('t1', 'transform', pipelineId),
    createPluginInstance('t2', 'transform', pipelineId),
    createPluginInstance('t3', 'transform', pipelineId),
    createPluginInstance('k1', 'sink', pipelineId),
    createPluginInstance('k2', 'sink', pipelineId),
  ];

  const edges = [
    createPluginEdge('s1', 't1', pipelineId),
    createPluginEdge('s2', 't2', pipelineId),
    createPluginEdge('t1', 't2', pipelineId),
    createPluginEdge('t2', 't3', pipelineId),
    createPluginEdge('t1', 'k1', pipelineId),
    createPluginEdge('t2', 'k1', pipelineId),
    createPluginEdge('t3', 'k2', pipelineId),
  ];

  return { instances, edges };
}