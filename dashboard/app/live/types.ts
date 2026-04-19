// import { Node } from "@xyflow/react";

// export type WorkerStatus = 'running' | 'busy' | 'down'

// export type WorkerPayload = {
//   id: string;
//   dataset: string;
//   load: number;
//   status: WorkerStatus;
// }

// export type Pipeline = {
//   id: string
//   name: string
//   workers: Worker[]
// }

// export type ClusterResponse = {
//   pipelines: Pipeline[]
// }

// export type WorkerNodeData = Node<WorkerPayload, 'worker'>;
// export type LayoutOptions = {
//   startX?: number;
//   startY?: number;
//   gapY?: number;
//   gapX?: number;
// };

import { Node, Edge } from '@xyflow/react';

/** ===== Domain ===== */

export type PipelineStatus = 'running' | 'idle';

export type Pipeline = {
  id: string;
  name: string;
  source: string;
  target: string;

  status: PipelineStatus;

  startedAt?: number;
  nextRunAt?: number;

  processed: number;
  errors: number;
  workerCount?: number;
};

/** ===== Node Data ===== */

export type DataSourceRole = 'source' | 'target';

export type DataSourceNodePayload = {
  name: string;
  role: DataSourceRole;
};


export type PipelineNodePayload = {
  id: string;
  name: string;
  status: PipelineStatus;
  startedAt?: number;
  nextRunAt?: number;
  processed: number;
  errors: number;
  workerCount?: number;
};

/** ===== React Flow Nodes ===== */

export type DataSourceNodeData = Node<DataSourceNodePayload, 'dataSource'>;
export type PipelineNodeData = Node<PipelineNodePayload, 'pipeline'>;

export type FlowNodeData = DataSourceNodeData | PipelineNodeData;

/** ===== Edges ===== */

export type FlowEdge = Edge;