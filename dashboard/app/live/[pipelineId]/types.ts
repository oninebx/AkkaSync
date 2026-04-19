import { PluginType } from '@/contracts/plugin/types';
import { Node, Edge } from '@xyflow/react';

export type PluginStatus = 'running' | 'idle' | 'succeeded' | 'failed';
export type PluginNodePayload = {
  id: string;
  name: string;
  type: PluginType;
  status: PluginStatus;
  stats: {
    processed: number;
    errors: number;
  };
}

export interface PluginAggregate {
  id: string;
  name: string;
  type: PluginType;
  status: PluginStatus;
  dependsOn: string[];
  stats: {
    processed: number;
    errors: number;
  };
}

export type PluginNodeData = Node<PluginNodePayload, PluginType>;
export type PluginNodeEdge = Edge;


//   id: string;
//   name: string;
//   pluginKey: string;
//   pipelineId: string;
//   type: PluginType;
//   status: 'idle' | 'running' | 'succeeded' | 'failed';
//   stats: {
//     processed: number;
//     errors: number;
//   };


// type PipelineStatus = "stopped" | "scheduled" | "running" | "idle" | "error";

// interface PipelineNodeData {
//   name: string;
//   status: PipelineStatus;
//   qps?: number;
//   latency?: number;
//   queue?: number;
//   errors?: number;
//   lastEventTime?: string;
// }

// interface PipelineNode {
//   id: string;
//   position: { x: number; y: number };
//   type?: string;
//   data: PipelineNodeData;
// }

// interface PipelineEdge {
//   id: string;
//   source: string;
//   target: string;
//   animated?: boolean;
//   style?: React.CSSProperties;
// }

// export type {
//   PipelineStatus,
//   PipelineNodeData,
//   // PipelineNode,
//   PipelineEdge
// }