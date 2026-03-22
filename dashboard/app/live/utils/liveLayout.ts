import { Node } from '@xyflow/react';

export type NodeLayout = Record<string, { x: number; y: number }>;

export const applyLiveLayout = (
  nodes: Node[],
  layout: NodeLayout | null
): Node[] => {
  if (!layout) return nodes;

  return nodes.map(n => ({
    ...n,
    position: layout[n.id] ?? n.position,
  }));
};