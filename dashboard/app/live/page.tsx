'use client';

import {
  ReactFlow,
  Background,
  useNodesState,
  useEdgesState,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';

import { buildGraph } from './utils';

import DataSourceNode from './components/DataSourceNode';
import PipelineNode from './components/PipelineNode';
import { useSelector } from 'react-redux';
import { selectClusterPipelines } from './selectors';

const nodeTypes = {
  dataSource: DataSourceNode,
  pipeline: PipelineNode,
};

export default function ClusterPage() {
  const pipelines = useSelector(selectClusterPipelines);
  const { nodes: initialNodes, edges: initialEdges } =
    buildGraph(pipelines);

  const [nodes, , onNodesChange] = useNodesState(initialNodes);
  const [edges, , onEdgesChange] = useEdgesState(initialEdges);

  return (
    <div className="w-full h-screen">
      <ReactFlow
        nodes={nodes}
        edges={edges}
        nodeTypes={nodeTypes}
        onNodesChange={onNodesChange}
        onEdgesChange={onEdgesChange}
        fitView
      >
        <Background />
      </ReactFlow>
    </div>
  );
}
