import { memo } from "react";
import { PipelineNodeData, PipelineStatus } from "../types";
import { Handle, Position } from "@xyflow/react";

interface PipelineNodeProps {
  data: PipelineNodeData;
}

const statusColors: Record<PipelineStatus, string> = {
  stopped: "bg-gray-300",
  scheduled: "bg-yellow-400",
  running: "bg-green-400",
  idle: "bg-blue-300",
  error: "bg-red-500",
};

export const PipelineNode = memo(({ data }: PipelineNodeProps) => {
  return (
    <div className={`p-3 rounded-lg shadow text-white ${statusColors[data.status]} w-40`}>
       <Handle type="target" position={Position.Left} />
      <div className="font-bold text-center">{data.name}</div>
      <div className="mt-2 text-sm">
        <div>Status: {data.status}</div>
        {data.qps !== undefined && <div>QPS: {data.qps}</div>}
        {data.latency !== undefined && <div>Latency: {data.latency} ms</div>}
        {data.queue !== undefined && <div>Queue: {data.queue}</div>}
        {data.errors !== undefined && <div>Errors: {data.errors}</div>}
      </div>
      <Handle type="source" position={Position.Right} />
    </div>
  );
});