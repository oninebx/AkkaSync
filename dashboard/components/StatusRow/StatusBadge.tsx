import React from 'react';
import { STATUS_CONFIG } from './statusConfig';
import { cn } from '@/lib/utils';
import StatusDot from './StatusDot';
import { StatusType } from '@/types/host';

interface StatusBadgeProps {
  status: StatusType;
  className?: string;
}

const StatusBadge = ({status, className}: StatusBadgeProps) => {
  const statusInfo = STATUS_CONFIG[status];
  return (
    <div className={cn('inline-flex items-center gap-1.5', className)}>
      <StatusDot color={statusInfo.color} />
      <span className="text-sm font-medium text-gray-700">{statusInfo.text}</span>
    </div>
  )
}

export default StatusBadge