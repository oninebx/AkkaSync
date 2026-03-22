'use client';

import { Node, Edge, ReactFlow, Controls, MiniMap, Background, addEdge, NodeChange, applyNodeChanges } from '@xyflow/react';
import React, { useCallback, useState } from 'react'
import { PipelineNode } from './components/PipelineNode';
import '@xyflow/react/dist/style.css';
import useLiveLayout from './hooks/useLiveLayout';
import { applyLiveLayout } from './utils/liveLayout';

type Props = {}

const initialNodes: Node[] = [
  { id: "1", type: "pipelineNode", data: { name: "Pipeline A", status: "running" }, position: { x: 0, y: 0 } },
  { id: "2", type: "pipelineNode", data: { name: "Pipeline B", status: "scheduled" }, position: { x: 250, y: 0 } },
  { id: "3", type: "pipelineNode", data: { name: "Pipeline C", status: "stopped" }, position: { x: 500, y: 0 } },
];

const initialEdges: Edge[] = [
  { id: "e1-2", source: "1", target: "2", animated: true },
  { id: "e2-3", source: "2", target: "3", animated: true },
];

const LivePage = (props: Props) => {
  const { saveLayout, loadLayout } = useLiveLayout();
  const [nodes, setNodes] = useState<Node[]>(() => {
    const layout = loadLayout();
    return applyLiveLayout(initialNodes, layout);
  });
  const [edges, setEdges] = useState<Edge[]>(initialEdges);

  

  const nodeTypes = {
    pipelineNode: PipelineNode,
  };

  const onConnect = useCallback((params: any) => setEdges((eds) => addEdge(params, eds)), []);
  const onNodesChange = useCallback(
  (changes: NodeChange[]) => {
    setNodes((nds) => {
      const updated = applyNodeChanges(changes, nds);
      saveLayout(updated);
      return updated;
    }
    );
  },
  []
);
  return (
    <div className='w-full h-full'>
      <ReactFlow
        nodes={nodes}
        edges={edges}
        nodeTypes={nodeTypes}
        onConnect={onConnect}
        onNodesChange={onNodesChange}
        fitView
      >
        <Controls />
        <MiniMap nodeStrokeColor={(n) => n.data.status === "error" ? "#f87171" : "#888"} />
        <Background gap={16} color="#aaa" />
      </ReactFlow>
    </div>
  )
}

export default LivePage;