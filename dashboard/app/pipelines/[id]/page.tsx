'use client'
import { useParams } from "next/navigation";
import { useSelector } from "react-redux";
import { selectPipelineTopology } from "../selectors";
import Breadcrumb from "@/components/Breadcrumb";
import { Background, Controls, MiniMap, ReactFlow } from "@xyflow/react";
import '@xyflow/react/dist/style.css';


function PipelineDetail() {
  const { id } = useParams<{id: string}>();

  const breadcrumbItems = [
    { label: 'Pipelines', href: '/pipelines' },
    { label: id, active: true },
  ];
  const topology = useSelector(state => selectPipelineTopology(state, id));

  return (
  <div className="h-screen flex flex-col p-8 max-w-7xl mx-auto">
    <Breadcrumb items={breadcrumbItems} />
    
    <header className="mb-4">
      <h1 className="text-3xl font-black text-slate-900 tracking-tight">
        Pipeline Topology
      </h1>
      <p className="text-slate-500 font-mono text-sm">
        Instance ID: {id}
      </p>
    </header>
    
    <div className="flex-1 min-h-0">
      <ReactFlow fitView>
        <Controls />
        <MiniMap nodeStrokeColor={(n) =>
          n.data.status === "error" ? "#f87171" : "#888"
        } />
        <Background gap={16} color="#aaa" />
      </ReactFlow>
    </div>
  </div>
);
}

export default PipelineDetail;