// app/pipelines/[id]/_components/PipelineTopologyBase.tsx
'use client'

import { useCallback, useMemo, useEffect } from "react";
import { useSelector } from "react-redux";
import { ReactFlow, Controls, MiniMap, Background, useNodesState, useEdgesState, Node, Edge } from "@xyflow/react";
import useLiveLayout from "@/infrastructure/storage/useLiveLayout";
import '@xyflow/react/dist/style.css';

interface PipelineTopologyBaseProps {
  id: string;
  title: string;
  selector: (state: any, id: string, savedLayout: any) => { nodes: any[]; edges: any[] };
  nodeTypes: Record<string, any>;
  path: string;
}

export default function PipelineTopologyBase({ id, title, selector, nodeTypes, path }: PipelineTopologyBaseProps) {
  const { loadLayout, saveLayout } = useLiveLayout(id, path);
  const savedLayout = useMemo(() => loadLayout(), [loadLayout]);

  const { nodes, edges } = useSelector((state) => selector(state, id, savedLayout));

  const [localNodes, setNodes, onNodesChange] = useNodesState(nodes);
  const [localEdges, setEdges, onEdgesChange] = useEdgesState(edges);

  useEffect(() => {
    setNodes(nodes);
    setEdges(edges);
  }, [nodes, edges, setNodes, setEdges]);

  const onNodeDragStop = useCallback(() => {
    saveLayout(localNodes);
  }, [saveLayout, localNodes]);

  return (
    <div className='h-screen flex flex-col p-8 mx-auto'>
      <header className='mb-4'>
        <h1 className='text-3xl font-black text-slate-900 tracking-tight'>{title}</h1>
        <p className='text-slate-500 font-mono text-sm'>Instance ID: {id}</p>
      </header>

      <div className='flex-1 min-h-0'>
        <ReactFlow
          fitView
          nodes={localNodes}
          edges={localEdges}
          nodeTypes={nodeTypes}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onNodeDragStop={onNodeDragStop}
        >
          <Controls />
          <MiniMap nodeStrokeColor={(n) => n.data.status === 'error' ? '#f87171' : '#888'} />
          <Background gap={16} color='#cbd5e1' />
        </ReactFlow>
      </div>
    </div>
  );
}