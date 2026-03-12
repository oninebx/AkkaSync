import React, { ReactNode } from 'react';
import { cn } from '@/lib/utils';

interface Props {
  color: string;
  className?: string;
  children?: ReactNode;
}

const CircleBadge = ({color, children, className}: Props) => {
  return (
    <div className={cn('inline-flex items-center gap-1.5', className)}>
      <span className={cn('w-3 h-3 rounded-full', color)}></span>
      {children && <span className="text-sm font-medium text-gray-700">{children}</span>}
    </div>
  )
}

export default CircleBadge;