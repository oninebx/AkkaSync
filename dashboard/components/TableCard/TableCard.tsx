import React from 'react'
import Card from '../Card'
import { Column, DisplayTable } from '../DisplayTable'

interface TableCardProps<T> {
  title: string,
  data: T[],
  columns: Column<T>[];
}

function TableCard<T>({title, data, columns}: TableCardProps<T>) {
  return (
    <Card>
        <h2 className="text-lg font-semibold text-gray-700 mb-4">{title}</h2>
        <DisplayTable columns={columns} data={data} />
    </Card>
  )
}

export default TableCard