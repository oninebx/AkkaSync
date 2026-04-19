// src/mocks/plugin-graph/generators.ts
import { PluginType } from "@/contracts/plugin/types";
import { PluginInstance, PluginEdge } from "@/features/execution/pluginGraph.type";

const statuses = ['idle', 'running', 'succeeded', 'failed'] as const;
type NodeStatus = typeof statuses[number];

function randomStatus(): NodeStatus {
  return statuses[Math.floor(Math.random() * statuses.length)];
}

function createStats() {
  return {
    processed: Math.floor(Math.random() * 20000),
    errors: Math.random() > 0.7 ? Math.floor(Math.random() * 100) : 0,
  };
}

function randomKey(length = 6) {
  return Math.random().toString(36).substring(2, 2 + length);
}

export function createPluginInstance(
  id: string,
  type: PluginType = 'transform',
  pipelineId?: string,
  pluginKey?: string,
  name?: string
): PluginInstance {
  return {
    id,
    name: name || `${type}-${id}`,
    type,
    pluginKey: pluginKey || randomKey(8),
    pipelineId: pipelineId || randomKey(6),
    status: randomStatus(),
    stats: createStats(),
  };
}

export function createPluginEdge(
  from: string,
  to: string,
  pipelineId?: string,
  id?: string
): PluginEdge {
  return {
    id: id || `edge-${from}-${to}`,
    from,
    to,
    pipelineId: pipelineId || randomKey(6),
  };
}