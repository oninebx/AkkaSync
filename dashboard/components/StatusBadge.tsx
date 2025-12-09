import { HostStatus } from '@/providers/SignalRProvider';
import React from 'react';

type Props = {
  status: HostStatus;
}

const StatusBadge = ({status}: Props) => {
  const map: Record<string, string> = {
    'online': 'bg-green-100 text-green-800',
    'offline': 'bg-red-100 text-red-800',
    'syncing': 'bg-blue-100 text-blue-800',
  };
  return (
    <span className={`px-2 py-1 rounded-full text-xs ${map[status]}`}>StatusBadge</span>
  )
}

export default StatusBadge;