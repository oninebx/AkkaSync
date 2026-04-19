'use client';
import { Handle, NodeProps, Position } from '@xyflow/react';
import { PipelineNodeData } from '../types';
import { useNow } from '../hooks/useNow';
import { useRouter } from 'next/navigation';



function format(ms: number): string {
  const s = Math.floor(ms / 1000);
  const m = Math.floor(s / 60);
  return `${m}m ${s % 60}s`;
}

export default function PipelineNode({
  data,
}: NodeProps<PipelineNodeData>) {

  const router = useRouter();
  const now = useNow();

  let timeText = '';

  if (data.status === 'running' && data.startedAt) {
    timeText = `Running ${format(now - data.startedAt)}`;
  } else if (data.status === 'idle' && data.nextRunAt) {
    const remain = data.nextRunAt - now;
    timeText =
      remain > 0 ? `Next in ${format(remain)}` : 'Starting...';
  }

  return (
    <div className="w-[260px] p-3 rounded-xl border bg-white shadow cursor-pointer"
      onClick={() => router.push(`/live/${data.id}`)}>
      <Handle type="target" position={Position.Left} />
      <div className="font-semibold text-sm">{data.name}</div>

      <div className="text-xs mt-1">
        {data.status === 'running' ? '🟢 Running' : '⚪ Idle'}
      </div>

      <div className="text-xs text-gray-500 mt-1">{timeText}</div>
      {data.status === 'running' && (
        <div className="text-xs mt-1 text-blue-600">
          Workers: {data.workerCount ?? 0}
        </div>
      )}

      <div className="grid grid-cols-2 gap-2 mt-2 text-xs">
        <div>Processed: {data.processed}</div>
        <div className="text-red-500">Errors: {data.errors}</div>
      </div>
      <Handle type="source" position={Position.Right} />
    </div>
  );
}