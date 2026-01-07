import { cn } from '@/lib/utils';
import React from 'react';

export interface Column<T = string> {
  key: keyof T;
  header: string;
  render?: (item: T) => React.ReactNode;
  className?: string;
}

interface TableProps<T> {
  columns: Column<T>[];
  data: T[];
  className?: string;
}

function DisplayTable<T>({columns, data, className}: TableProps<T>){

  return (
    <div className={cn('overflow-x-auto', className)}>
      <table className="min-w-full text-sm">
        <thead>
          <tr className="bg-gray-100 text-gray-700">
            {columns.map((column) => (
              <th key={String(column.key)} className={cn('py-2 px-4 text-left', column.className)}>{column.header}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data?.map((item, rowIndex) => (
            <tr key={rowIndex} className="border-b last:border-none">
              {columns.map((column) => (
                <td key={String(column.key)} className={cn('py-2 px-4', column.className)}>
                  {column.render ? column.render(item) : item[column.key] as React.ReactNode}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default DisplayTable;