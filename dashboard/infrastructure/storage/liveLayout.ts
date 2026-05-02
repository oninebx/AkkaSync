import { Node } from '@xyflow/react';

export type NodeLayout = Record<string, { x: number; y: number }>;

const applyLiveLayout = (
  nodes: Node[],
  layout: NodeLayout | null
): Node[] => {
  if (!layout) return nodes;

  return nodes.map(n => ({
    ...n,
    position: layout[n.id] ?? n.position,
  }));
};

const getEffectivePosition = (
  nodeId: string, 
  defaultPos: { x: number, y: number }, 
  savedLayout: NodeLayout | null
) => {
  return savedLayout?.[nodeId] || defaultPos;
};

export {
  applyLiveLayout,
  getEffectivePosition
}