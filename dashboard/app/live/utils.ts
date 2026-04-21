// import { Position } from "@xyflow/react";
// import { LayoutOptions, WorkerNodeData } from "./types";

// export function layoutWorkers(
//   nodes: WorkerNodeData[],
//   options: LayoutOptions = {}
// ): WorkerNodeData[] {
//   const {
//     startX = 0,
//     startY = 0,
//     gapY = 80,
//   } = options;

//   return nodes.map((node, index) => {
//     return {
//       ...node,
//       position: {
//         x: startX,
//         y: startY + index * gapY,
//       },
//       sourcePosition: Position.Right,
//       targetPosition: Position.Left,
//     };
//   });
// }

import { PluginDefinition, PluginRun } from '@/features/pipeline/pipeline.types';
import { FlowNodeData, FlowEdge, Pipeline } from './types';

type BuildResult = {
  nodes: FlowNodeData[];
  edges: FlowEdge[];
};

export function buildGraph(pipelines: Pipeline[]): BuildResult {
  const nodes: FlowNodeData[] = [];
  const edges: FlowEdge[] = [];

  const sourceMap = new Map<string, string>();
  const targetMap = new Map<string, string>();

  let row = 0;

  pipelines.forEach((p) => {
    const y = row * 140;

    /** ---------- Source ---------- */
    let sourceId = sourceMap.get(p.source);
    if (!sourceId) {
      sourceId = `source-${p.source}`;
      sourceMap.set(p.source, sourceId);

      nodes.push({
        id: sourceId,
        type: 'dataSource',
        position: { x: 0, y },
        data: { name: p.source, role: 'source' },
      });
    }

    /** ---------- Pipeline ---------- */
    const pipelineId = `pipeline-${p.id}`;

    nodes.push({
      id: pipelineId,
      type: 'pipeline',
      position: { x: 260, y },
      data: {
        id: p.id,
        name: p.name,
        status: p.status,
        startedAt: p.startedAt,
        nextRunAt: p.nextRunAt,
        processed: p.processed,
        errors: p.errors,
        workerCount: p.workerCount,
      },
    });

    /** ---------- Target ---------- */
    let targetId = targetMap.get(p.target);
    if (!targetId) {
      targetId = `target-${p.target}`;
      targetMap.set(p.target, targetId);

      nodes.push({
        id: targetId,
        type: 'dataSource',
        position: { x: 520, y },
        data: { name: p.target, role: 'target' },
      });
    }

    /** ---------- Edges ---------- */
    edges.push(
      {
        id: `${sourceId}-${pipelineId}`,
        source: sourceId,
        target: pipelineId,
      },
      {
        id: `${pipelineId}-${targetId}`,
        source: pipelineId,
        target: targetId,
      }
    );

    row++;
  });

  return { nodes, edges };
}