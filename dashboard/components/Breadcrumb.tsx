import React from 'react';
import Link from 'next/link';
import { ChevronRight, Home } from 'lucide-react';
import { cn } from '@/lib/utils';

export interface BreadcrumbItem {
  label: string;
  href?: string;
  active?: boolean;
}

interface BreadcrumbProps {
  items: BreadcrumbItem[];
  className?: string;
}

const Breadcrumb = ({ items, className }: BreadcrumbProps) => {
  return (
    <nav 
      aria-label="Breadcrumb" 
      className={cn("flex items-center text-sm font-medium mb-6", className)}
    >
      <ol className="flex items-center space-x-2">
        {/* Home Icon as Default Root */}
        <li>
          <Link 
            href="/" 
            className="text-slate-400 hover:text-indigo-600 transition-colors"
          >
            <Home size={16} />
          </Link>
        </li>

        {items.map((item, index) => (
          <li key={index} className="flex items-center space-x-2">
            <ChevronRight size={14} className="text-slate-300 flex-shrink-0" />
            
            {item.active || !item.href ? (
              <span className="text-slate-900 font-bold truncate max-w-[200px]">
                {item.label}
              </span>
            ) : (
              <Link
                href={item.href}
                className="text-slate-500 hover:text-indigo-600 transition-colors whitespace-nowrap"
              >
                {item.label}
              </Link>
            )}
          </li>
        ))}
      </ol>
    </nav>
  );
};

export default Breadcrumb;