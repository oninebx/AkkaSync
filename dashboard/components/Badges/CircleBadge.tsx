import React from 'react';
import { cn } from '@/lib/utils';

interface Props {
  color: string;
  text?: string;
  className?: string;
}

const CircleBadge = ({color, text, className}: Props) => {
  return (
    <div className={cn('inline-flex items-center gap-1.5', className)}>
      <span className={cn('w-3 h-3 rounded-full', color)}></span>
      {text && <span className="text-sm font-medium text-gray-700">{text}</span>}
    </div>
  )
}

export default CircleBadge;