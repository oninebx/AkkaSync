// app/pipelines/[id]/_components/PipelineTopologyBase.tsx
'use client'

import { useCallback, useMemo, useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { ReactFlow, Controls, MiniMap, Background, useNodesState, useEdgesState, Node, Edge, Panel } from "@xyflow/react";
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

  const { nodes: remoteNodes, edges: remoteEdges } = useSelector((state) => selector(state, id, savedLayout));

  const [localNodes, setNodes, onNodesChange] = useNodesState(remoteNodes);
  const [localEdges, setEdges, onEdgesChange] = useEdgesState(remoteEdges);


  useEffect(() => {
    if (!remoteNodes || remoteNodes.length === 0) {
      return;
    }

    const nextNodes = remoteNodes.map(rn => {
      const local = localNodes.find(ln => ln.id === rn.id);
      return {
        ...rn,
        position: local?.position ?? rn.position 
      };
    });

   
    const nodeIdSet = new Set(nextNodes.map(n => n.id));
    setNodes(nextNodes);
    
    const timer = setTimeout(() => {
      setEdges(currentEdges => {
        const edgeMap = new Map(currentEdges.map(e => [e.id, e]));
        remoteEdges.forEach(re => {
          edgeMap.set(re.id, {
          ...edgeMap.get(re.id), 
          ...re
          });
        });
        const nextdges = Array.from(edgeMap.values());
        // .filter(e => 
        //   nodeIdSet.has(e.source) && nodeIdSet.has(e.target)
        // );
        return nextdges;
      });
    }, 100);
    return () => clearTimeout(timer);

  }, [remoteNodes, remoteEdges]); 



  const onNodeDragStop = useCallback(() => {
    saveLayout(localNodes);
  }, [saveLayout, localNodes]);


const [refreshKey, setRefreshKey] = useState(0);

  // 手动刷新函数
  const forceRefresh = useCallback(() => {
    setRefreshKey(prev => prev + 1);
  }, []);


  return (
    <div className='h-screen flex flex-col p-8 mx-auto'>
      <header className='mb-4'>
        <h1 className='text-3xl font-black text-slate-900 tracking-tight'>{title}</h1>
        <p className='text-slate-500 font-mono text-sm'>Instance ID: {id}</p>
      </header>

      <div className='flex-1 min-h-0' key={refreshKey}>
        <ReactFlow
          fitView
          nodes={localNodes}
          edges={localEdges}
          nodeTypes={nodeTypes}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onNodeDragStop={onNodeDragStop}
          onlyRenderVisibleElements={false}
        >
          <Controls />
          <MiniMap nodeStrokeColor={(n) => n.data.status === 'error' ? '#f87171' : '#888'} />
          <Background gap={16} color='#cbd5e1' />
          <Panel position="top-right" className="flex gap-2">
      <button 
        onClick={forceRefresh}
        className="px-3 py-1 bg-white border border-slate-300 rounded shadow-sm hover:bg-slate-50 text-sm flex items-center gap-2"
      > Refresh
        {/* <RefreshIcon className="w-4 h-4" /> 修复连线 */}
      </button>
    </Panel>
        </ReactFlow>
      </div>
    </div>
  );
}