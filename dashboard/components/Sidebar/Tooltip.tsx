import React from "react";

interface TooltipProps {
  text: string;
  children: React.ReactNode;
}

export const Tooltip = ({ text, children }: TooltipProps) => {
  return (
    <div className="relative group flex items-center">
      {children}
      <span
        className="
          absolute left-full ml-3 px-2 py-1 rounded bg-gray-800 text-white text-xs opacity-0
          group-hover:opacity-100 transition-opacity whitespace-nowrap z-50
        "
      >
        {text}
      </span>
    </div>
  );
};
