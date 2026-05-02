import { Handle, NodeProps, Position } from '@xyflow/react';
import { ConnectorNodeData } from '../types';

export default function ConnectorNode({ data }: NodeProps<ConnectorNodeData>) {
  const isSource = data.role === 'SOURCE';
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