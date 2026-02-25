import React from 'react'
import {TableCard } from '@/components/TableCard';
import { Column } from '@/components/DisplayTable/DisplayTable';
import { CircleBadge } from '@/components/Badges';
import PluginActionContainer from './PluginActionContainer';
import { usePluginActions } from './usePluginActions';

export type PluginStatus = 'loaded' | 'unloaded' | 'error';

export interface PluginListItem {
  id: string;
  name: string;
  type: 'source' | 'transform' | 'sink' | 'store';
  version?: string;
  usedByCount?: number;
  status: PluginStatus;
  actions?: string[];
}

type Props = {
  data: PluginListItem[];
}

const pluginColumns = (
  doUpload: (e: React.MouseEvent<HTMLButtonElement>) => void,
  doRemove: (e: React.MouseEvent<HTMLButtonElement>) => void
) => [
  { key: "name", header: "Plugin" },
  { key: "type", header: "Type", render: (item) => <span className="capitalize">{item.type}</span> },
  { key: "version", header: "Version", defaultValue: '-' },
  { key: "usedByCount", header: "Used By", defaultValue: 0 },
  { key: "status", header: "Status", render: (item) => <CircleBadge color={item.status === 'loaded' ? 'bg-green-500' : item.status === 'unloaded' ? 'bg-gray-500' : 'bg-red-500'} /> },
  { key: "actions", header: "Actions", render: ({id, status}) => <PluginActionContainer id={id} status={status} handleUpload={doUpload} handleRemove={doRemove} /> },
] as Column<PluginListItem>[];

const PluginTable = (props: Props) => {
  const doUpload = (id: string, file: File) => {
    console.log('Uploading', id, file);
  }
  const doRemove = (id: string) => {
    console.log('Removing', id);
  }

  const { inputRef, handleUpload, handleRemove, handleFileChange } = usePluginActions({ upload: doUpload, remove: doRemove });
  const columns = pluginColumns(handleUpload, handleRemove);
  
  return (
    <>
      <TableCard title='Sync Plugins' columns={columns} data={props.data} />
      <input type='file' className='hidden' onChange={handleFileChange} ref={inputRef} />
    </>
    
  )
}

export default PluginTable;