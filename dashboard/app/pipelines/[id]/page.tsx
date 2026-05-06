'use client'
import { QueryResponse, useSignalRQuery } from "@/infrastructure/realtime/SignalRProvider";
// import { useParams } from "next/navigation";
// import { useSelector } from "react-redux";
// import Breadcrumb from "@/components/Breadcrumb";
// import { Background, Controls, MiniMap, NodeChange, ReactFlow, useEdgesState, useNodesState } from "@xyflow/react";
// import '@xyflow/react/dist/style.css';
// import { selectPipelineTopology } from "./selectors";
// import { ConnectorNode, PluginNode } from "./components";
// import { useCallback, useMemo } from "react";
// import useLiveLayout from "@/infrastructure/storage/useLiveLayout";
// import { QueryResponse, useSignalRQuery } from "@/infrastructure/realtime/SignalRProvider";

import { ConnectorNode, PluginNode } from "./_components";
import PipelineTopologyBase from "./_components/PipelineTopologyBase";
import { selectPipelineTopology } from "./selectors";
import { useParams, usePathname } from "next/navigation";

// const nodeTypes = {
//   connector: ConnectorNode,
//   plugin: PluginNode
// };

// function PipelineDetail() {
//   const { id } = useParams<{id: string}>();

//   const breadcrumbItems = [
//     { label: 'Pipelines', href: '/pipelines' },
//     { label: id, active: true },
//   ];

//   const { data } = useSignalRQuery<QueryResponse>('CheckForUpdates');
//   if(data && !data.success){
//     alert(data.message);
//   }

//   const { loadLayout, saveLayout } = useLiveLayout(id);
//   const savedLayout = useMemo(() => loadLayout(), [loadLayout]);

//   const {nodes, edges} = useSelector(state => selectPipelineTopology(state, id, savedLayout));

//   const [localNodes, setNodes, onNodesChange] = useNodesState(nodes);
//   const [localEdges, setEdges, onEdgesChange] = useEdgesState(edges);

//   const onNodeDragStop = useCallback(() => {
//     saveLayout(localNodes);
//   }, [saveLayout, localNodes]);

//   return (
//   <div className="h-screen flex flex-col p-8 mx-auto">
//     <Breadcrumb items={breadcrumbItems} />
    
//     <header className="mb-4">
//       <h1 className="text-3xl font-black text-slate-900 tracking-tight">
//         Pipeline Topology
//       </h1>
//       <p className="text-slate-500 font-mono text-sm">
//         Instance ID: {id}
//       </p>
//     </header>
    
//     <div className="flex-1 min-h-0">
//       <ReactFlow 
//         fitView
//         nodeTypes={nodeTypes}
//         nodes={localNodes}
//         edges={localEdges}
//         onNodesChange={onNodesChange}
//         onNodeDragStop={onNodeDragStop}>
//         <Controls />
//         <MiniMap nodeStrokeColor={(n) =>
//           n.data.status === "error" ? "#f87171" : "#888"
//         } />
//         <Background gap={16} color="#aaa" />
//       </ReactFlow>
//     </div>
//   </div>
// );
// }

// export default PipelineDetail;

// app/pipelines/[id]/blueprint/page.tsx

const nodeTypes = { connector: ConnectorNode, plugin: PluginNode };

export default function PipelinePage() {
  const { id } = useParams<{id: string}>();
  const pathName = usePathname();
  const { data } = useSignalRQuery<QueryResponse>('CheckForUpdates');
  return (
    <PipelineTopologyBase
      id={id}
      title="Pipeline Blueprint"
      selector={selectPipelineTopology}
      nodeTypes={nodeTypes}
      path={pathName}
    />
  );
}