type PipelineStatus = "stopped" | "scheduled" | "running" | "idle" | "error";

interface PipelineNodeData {
  name: string;
  status: PipelineStatus;
  qps?: number;
  latency?: number;
  queue?: number;
  errors?: number;
  lastEventTime?: string;
}

// interface PipelineNode {
//   id: string;
//   position: { x: number; y: number };
//   type?: string;
//   data: PipelineNodeData;
// }

interface PipelineEdge {
  id: string;
  source: string;
  target: string;
  animated?: boolean;
  style?: React.CSSProperties;
}

export type {
  PipelineStatus,
  PipelineNodeData,
  // PipelineNode,
  PipelineEdge
}