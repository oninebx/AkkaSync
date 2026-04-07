import { PluginInstance } from '@/features/plugin-graph/pluginGraph.type';
import { cn } from '@/lib/utils';
import { Handle, Position } from '@xyflow/react';
import React from 'react'

type Props = {
  data: PluginInstance;
  color: string;
  label: string;
  icon: string;
  isSource: boolean;
  isTarget: boolean;
}

function getStatusTextColor(status: PluginInstance['status']) {
  switch (status) {
    case 'running':
    case 'succeeded':
      return 'text-green-600';
    case 'failed':
      return 'text-red-600';
    default:
      return 'text-gray-500';
  }
}

function BaseNode({data, color, label, icon, isSource, isTarget}: Props) {
  return (
    <div 
      className='min-w-[220px] rounded-xl border-2 bg-white shadow-md overflow-hidden text-sm transition hover:shadow-lg'
      style={{ borderColor: color }}>
      <div className='flex items-center p-3'>
        <div 
          className='w-3 h-3 rounded mr-2' 
          style={{ backgroundColor: color }} />
        <strong>{data.id}</strong>
      </div>
      <div className='px-3 pb-2 text-gray-500'>ID: {data.id}</div>
      <div className='h-px bg-gray-200' />
      <div className='p-3'>
        <div className={cn('capitalize font-semibold', getStatusTextColor(data.status))}>
          {data.status}
        </div>
        <div className="mt-1.5">Processed: {data.stats.processed.toLocaleString()}</div>

        {data.stats.errors > 0 && (
          <div className="text-red-500">Errors: {data.stats.errors}</div>
        )}
      </div>
      {isTarget && <Handle type="target" position={Position.Left} />}
      {isSource && <Handle type="source" position={Position.Right} />}
    </div>
  )
}

export default BaseNode;