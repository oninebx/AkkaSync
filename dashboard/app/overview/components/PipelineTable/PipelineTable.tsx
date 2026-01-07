import { Column } from '@/components/DisplayTable';
import { TableCard } from '@/components/TableCard';
import React from 'react';
import { OverviewPipeline } from './usePipelines';

const pipelineColumns: Column<OverviewPipeline>[] = [
  { key: "name", header: "Pipeline" },
  { key: 'duration', header: 'Duration' },
  { key: "schedule", header: "Schedule" },
  { key: "lastRun", header: "Last Run" },
  { key: 'nextRun', header: 'Next Run'}
];

interface Props {
  data: OverviewPipeline[];
}

const PipelineTable = ({ data }: Props) => {
  return (
    <TableCard title='Active Pipelines Overview' columns={pipelineColumns} data={data} />
  )
}

export default PipelineTable;