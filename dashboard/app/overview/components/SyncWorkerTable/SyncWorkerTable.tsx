import { PillBadge, ProgressBadge } from '@/components/Badges';
import { Column } from '@/components/DisplayTable';
import { TableCard } from '@/components/TableCard';
import React from 'react';
import { OverviewSyncWorker } from './types';
import { statusColor, useSyncWorkers } from './useSyncWorkers';

const syncWorkerColumns: Column<OverviewSyncWorker>[] = [
  { key: 'name', header: 'Pipeline' },
  { key: "runId", header: "Run" },
  { key: 'status', header: 'Status', render: worker => <PillBadge label={worker.status} bgColor={statusColor[worker.status]} /> },
  { key: "stage", header: "Stage" },
  { key: 'progress', header: 'Progress', render: worker => <ProgressBadge progress={worker.progress} bgColor={statusColor[worker.status]} /> }
];

const SyncWorkerTable = () => {
  const data = useSyncWorkers();
  return (
    <TableCard title='Active Sync Runs' columns={syncWorkerColumns} data={data} />
  )
}

export default SyncWorkerTable;