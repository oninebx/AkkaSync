import React from 'react'
import StatusBadge from './StatusBadge';
import { cn } from '@/lib/utils';
import { StatusType } from '@/types/host';

interface Props {
  label: string;
  status: StatusType;
  labelWidth?: string;
}

const StatusRow = ({ label, status, labelWidth = 'w-24' }: Props) => {
  return (
    <div className='flex items-center mt-2 space-x-2'>
      <span className={cn('font-medium text-gray-800 whitespace-nowrap text-sm', labelWidth)}>{label}:</span>
      <StatusBadge status={status} />
    </div>
  )
}

export default StatusRow;