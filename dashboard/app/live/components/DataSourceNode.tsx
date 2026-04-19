import { Handle, NodeProps, Position } from '@xyflow/react';
import { DataSourceNodeData } from '../types';

export default function DataSourceNode({ data }: NodeProps<DataSourceNodeData>) {
  const isSource = data.role === 'source';
  return (
    <div
      className={`
        px-3 py-2 border rounded text-xs
        ${isSource ? 'bg-blue-50' : 'bg-green-50'}
      `}
    >
      {isSource ? '📥' : '📤'} {data.name}

      {isSource ? (
        <Handle type="source" position={Position.Right} />
      ) : (
        <Handle type="target" position={Position.Left} />
      )}
    </div>
  );
}