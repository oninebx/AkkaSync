'use client';

import { Node, Edge, ReactFlow, Controls, MiniMap, Background, addEdge, NodeChange, applyNodeChanges } from '@xyflow/react';
import React, { useCallback, useState } from 'react'
import { PipelineNode } from './components/PipelineNode';
import useLiveLayout from './hooks/useLiveLayout';
import { applyLiveLayout } from './utils/liveLayout';
import { SinkNode, SourceNode, TransformNode } from './components/FlowNode';
import { useSelector } from 'react-redux';
import { selectLayoutedFlowData } from './selectors';
import '@xyflow/react/dist/style.css';

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

  const {nodes, edges } = useSelector(selectLayoutedFlowData);

  const { saveLayout, loadLayout } = useLiveLayout();
  const [layoutNodes, setLayoutNodes] = useState<Node[]>(() => {
    const layout = loadLayout();
    return applyLiveLayout(nodes, layout);
  });
  const [layoutEdges, setLayoutEdges] = useState<Edge[]>(edges);


  const nodeTypes = {
    pipelineNode: PipelineNode,
    source: SourceNode,
    transform: TransformNode,
    sink: SinkNode,
  };

  const onConnect = useCallback((params: any) => setLayoutEdges((eds) => addEdge(params, eds)), []);
  const onNodesChange = useCallback(
  (changes: NodeChange[]) => {
    setLayoutNodes((nds) => {
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
        nodes={layoutNodes}
        edges={layoutEdges}
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