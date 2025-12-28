import { cn } from '@/lib/utils';
import React from 'react';
import { BgColor } from '.';

interface Props {
  bgColor: BgColor,
  label?: string
}

const PillBadge = ({bgColor, label}: Props) => {
  return (
    <span className={cn('inline-block px-2 py-1 rounded text-xs font-medium text-white', bgColor)}>
      {label ?? bgColor}
    </span>
  )
}

export default PillBadge;