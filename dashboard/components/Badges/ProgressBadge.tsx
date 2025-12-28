import React from 'react'
import { BgColor } from './types';
import { cn } from '@/lib/utils';

interface Props {
  progress: number,
  bgColor: BgColor
}

const ProgressBadge = ({ progress, bgColor }: Props) => {
  return (
    <div className="w-full bg-gray-200 rounded-full h-2">
            <div
              className={cn("h-2 rounded-full", bgColor)}
              style={{ width: `${progress}%` }}
            />
          </div>
  )
}

export default ProgressBadge;