import { cn } from '@/lib/utils';
import React from 'react'

type ButtonColor =  'blue' | 'red' | 'green' | 'yellow'

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  color: ButtonColor;
}

const colorMap: Record<ButtonColor, string> = {
  blue: 'bg-blue-500 hover:bg-blue-600',
  red: 'bg-red-500 hover:bg-red-600',
  green: 'bg-green-500 hover:bg-green-600',
  yellow: 'bg-yellow-500 hover:bg-yellow-600',
}

const ActionButton = ({ children, color, className, disabled, ...props }: ButtonProps) => {
  const extraClasses = disabled ? 'bg-gray-400 hover:bg-gray-400 cursor-not-allowed' : 'hover:cursor-pointer';
  return (
    <button
      {...props}
      className={cn(colorMap[color], className, extraClasses, 'text-white px-3 py-1 rounded text-sm transition-colors')}
    >
      {children}
    </button>
  )
}

export default ActionButton;