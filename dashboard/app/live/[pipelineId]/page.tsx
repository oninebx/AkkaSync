'use client';
import { Node, Edge, ReactFlow, Controls, MiniMap, Background, addEdge, NodeChange, applyNodeChanges } from '@xyflow/react';
import React, { useCallback, useEffect, useState } from 'react'
import useLiveLayout from '../../../infrastructure/storage/useLiveLayout';
import { SinkNode, SourceNode, TransformNode } from './components/FlowNode';
import { useSelector } from 'react-redux';
import { selectPluginGraph } from './selectors';
import '@xyflow/react/dist/style.css';
import { useParams } from 'next/navigation';
import { RootState } from '@/store';
import { applyLiveLayout } from '@/infrastructure/storage/liveLayout';


const LivePage = () => {
  const { pipelineId } = useParams<{pipelineId: string}>();

  const {nodes, edges } = useSelector((state: RootState) => selectPluginGraph(state, pipelineId));

  const { saveLayout, loadLayout } = useLiveLayout(pipelineId);
  const [layoutNodes, setLayoutNodes] = useState<Node[]>(() => {
    const layout = loadLayout();
    return applyLiveLayout(nodes, layout);
  });
  const [layoutEdges, setLayoutEdges] = useState<Edge[]>(edges);

  useEffect(() => {
    const layout = loadLayout();
    setLayoutNodes(applyLiveLayout(nodes, layout));
  }, [nodes, loadLayout]);

  useEffect(() => {
    setLayoutEdges(edges);
    
  }, [edges]);

  const nodeTypes = {
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