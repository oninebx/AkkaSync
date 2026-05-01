import { Column } from '@/components/DisplayTable';
import { TableCard } from '@/components/TableCard';
import React from 'react';
import { PipelineVM } from '@/app/overview/types';

const pipelineColumns: Column<PipelineVM>[] = [
  { key: "name", header: "Pipeline" },
  { key: 'totalRuns', header: 'Runs' },
  { key: "totalProcessed", header: "Total Processed" },
  { key: "totalErrors", header: "Total Errors" }
];

interface Props {
  data: PipelineVM[];
}

const PipelineTable = ({ data }: Props) => {
  return (
    <TableCard title='Active Pipelines Overview' columns={pipelineColumns} data={data} />
  )
}

export default PipelineTable;