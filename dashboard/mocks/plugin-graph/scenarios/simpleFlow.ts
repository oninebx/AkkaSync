// src/mocks/plugin-graph/scenarios/simpleFlow.ts
import { createPluginInstance, createPluginEdge } from "../generators";

export function createSimpleFlow() {
  const pipelineId = "pipeline-001";

  const instances = [
    createPluginInstance('s1', 'source', pipelineId),
    createPluginInstance('t1', 'transform', pipelineId),
    createPluginInstance('t2', 'transform', pipelineId),
    createPluginInstance('k1', 'sink', pipelineId),
  ];

  const edges = [
    createPluginEdge('s1', 't1', pipelineId),
    createPluginEdge('t1', 't2', pipelineId),
    createPluginEdge('t2', 'k1', pipelineId),
  ];

  return { instances, edges };
}