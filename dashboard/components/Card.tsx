import { cn } from '@/lib/utils';
import React from 'react';

interface CardProps {
  children?: React.ReactNode;
  width?: string;
  height?: string;
  padding?: string;
  className?: string;
}

const Card = ({children, width = 'w-full', height = 'h-auto', padding='p-4', className = ''}: CardProps) => {
  return (
    <div className={cn('bg-white shadow rounded-lg', width, height, padding, className)}>
      {children}
    </div>
  )
}

export default Card;