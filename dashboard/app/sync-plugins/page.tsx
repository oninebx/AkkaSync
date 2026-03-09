'use client';
import React from 'react';
import PluginTable from './components/PluginTable/PluginTable';
import { RefreshCcw } from 'lucide-react';
import { useSelector } from 'react-redux';
import { selectPlugins } from '@/features/plugin-hub/plugin-hub.selectors';
import { PluginListItem } from '@/features/plugin-hub/plugin-hub.types';
import { useSignalRQuery } from '@/providers/SingalRProvider';
import { QueryResponse } from '@/providers/SingalRProvider/types';

const mockPlugins: PluginListItem[] = [
  { id: 'source.plugina', name: 'Plugin A', type: 'source', version: '1.0.0', usedByCount: 2, status: 'loaded' },
  { id: 'transform.pluginb', name: 'Plugin B', type: 'transform', status: 'unloaded' },
  { id: 'sink.pluginc', name: 'Plugin C', type: 'sink', version: '1.2.0', usedByCount: 1, status: 'error' },
];

/*
const { queryInvoke } = useSignalRInvoke<PingResponse>();
  
  const handleClick = async () => {
    try{
      const data = await queryInvoke('CheckVersions', { Value: 'ping' }, true);
      console.log(data);
    }catch(err){
      console.log(err);
    }
  }
  // const handleReset = () => {
    
  // }
*/

const PluginsPage = () => {
  const handleScan = () => {};

  const { data } = useSignalRQuery<QueryResponse>('CheckVersions');
  console.log(data);
  const plugins = useSelector(selectPlugins);
  // console.log(plugins);

  return (
    <div className='p-6 bg-gray-50 min-h-screen'>
      <div className='flex justify-between items-center mb-6'>
        <h2 className='text-2xl font-bold text-gray-900'>Plugins</h2>
        <button onClick={handleScan} className='bg-primary text-white font-semibold px-4 py-2 rounded hover:bg-primary-dark transition-colors'>
          <RefreshCcw size={16} />
        </button>
      </div>
      <PluginTable data={plugins} />
    </div>
  )
}

export default PluginsPage;