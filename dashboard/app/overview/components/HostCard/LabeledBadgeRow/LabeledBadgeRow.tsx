import React from 'react'
import { cn } from '@/lib/utils';
import { CircleBadge } from '@/components/Badges';

interface Props {
  label: string;
  color: string;
  text: string;
  labelWidth?: string;
}

const LabeledBadgeRow = ({ label, color, text, labelWidth = 'w-24' }: Props) => {
  return (
    <div className='flex items-center mt-2 space-x-2'>
      <span className={cn('font-medium text-gray-800 whitespace-nowrap text-sm', labelWidth)}>{label}:</span>
      <CircleBadge color={color} text={text} />
    </div>
  )
}

export default LabeledBadgeRow;