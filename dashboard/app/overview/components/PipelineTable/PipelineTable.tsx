import { Column } from '@/components/DisplayTable';
import { TableCard } from '@/components/TableCard';
import React from 'react';
import { OverviewPipeline, usePipelines } from './usePipelines';

const pipelineColumns: Column<OverviewPipeline>[] = [
  { key: "name", header: "Pipeline" },
  { key: 'duration', header: 'Duration' },
  { key: "schedule", header: "Schedule" },
  { key: "lastRun", header: "Last Run" },
];

// type Props = {}

const PipelineTable = () => {
  const data = usePipelines();
  return (
    <TableCard title='Active Pipelines Overview' columns={pipelineColumns} data={data} />
  )
}

export default PipelineTable;