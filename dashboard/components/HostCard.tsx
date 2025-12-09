import React from 'react';
import StatusBadge from './StatusBadge';
import { Host } from '@/types/dashboard';

const HostCard = ({ name, status }: Omit<Host, 'id' | 'signalRHubUrl'>) => {
  return (
    <div className='bg-white rounded-lg p-4 shadow-sm'>
      <div className='flex justify-between items-start'>
        <div>
          <div className='text-base font-semibold'>{name}</div>
        </div>
        <div className='text-right'>
          <StatusBadge status={status} />
        </div>
      </div>
    </div>
  )
}

export default HostCard;