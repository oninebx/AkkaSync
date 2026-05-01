import React from 'react'
import {TableCard } from '@/components/TableCard';
import { Column } from '@/components/DisplayTable/DisplayTable';
import { CircleBadge } from '@/components/Badges';
import PluginActionContainer from './PluginActionContainer';
import { PluginVM } from '@/app/sync-plugins/types';
import VersionCell from './VersionCell';
import NameCell from './NameCell';



type Props = {
  data: PluginVM[];
  handleUpdate: (e: React.MouseEvent<HTMLButtonElement>) => void;
  updatingId: string | null;
}

const pluginColumns = (
  doUpdate: (e: React.MouseEvent<HTMLButtonElement>) => void,
  updatingId: string | null
) => [
    { key: "name", header: "Plugin", render: (item) => <NameCell name={item.name} installedVersion={item.installedVersion} latestVersion={item.latestversion} /> },
    { key: "type", header: "Type", render: (item) => <span className="capitalize">{item.kind}</span> },
    { key: "version", header: "Version", render: (item) => <VersionCell latestVersion={item.latestversion} installedVersion={item.installedVersion} />},
    { key: "usedByCount", header: "Used By", defaultValue: 0 },
    { key: "actions", header: "Actions", render: (item) => <PluginActionContainer id={item.id} /*latestVersion={item.latestversion} installedVersion={item.installedVersion} */disabled={updatingId === item.id} canUpgrade={item.status === 'updateAvailable'} handleUpdate={doUpdate} /> },
  ] as Column<PluginVM>[];

  const PluginTable = ({data, handleUpdate, updatingId}: Props) => {

  const columns = pluginColumns(handleUpdate, updatingId);
  
  return (
    <>
      <TableCard title='Sync Plugins' columns={columns} data={data} />
    </>
    
  )
}

export default PluginTable;