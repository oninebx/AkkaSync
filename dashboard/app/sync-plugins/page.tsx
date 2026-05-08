'use client';
import React, { useEffect, useState } from 'react';
import PluginTable from './components/PluginTable/PluginTable';
import { RefreshCcw } from 'lucide-react';
import { useSelector } from 'react-redux';
import { selectPluginPackages } from './selectors';
import { QueryResponse, useSignalRInvoke, useSignalRQuery } from '@/infrastructure/realtime/SignalRProvider';

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

  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const handleScan = () => {};

  const { data } = useSignalRQuery<QueryResponse>('CheckForUpdates');
  if(data && !data.success){
    alert(data.message);
  }
  const plugins = useSelector(selectPluginPackages);

  const { queryInvoke} = useSignalRInvoke<QueryResponse>();
  const handleUpdate = async (e: React.MouseEvent<HTMLButtonElement>) => {
    const id = e.currentTarget.dataset.id;
    if(id) {
      try{
        const plugin = plugins.find(p => p.id === id);
        if(plugin?.installedVersion !== plugin?.latestversion){
          const response = await queryInvoke('CheckoutPlugin', { Id: id }, true);
          setUpdatingId(id);
          
          if(!response.success){
            setUpdatingId(null);
            alert(response.message);
          }
        }
        
      }catch(err){
        console.error(err);
      }
    }
  }

 useEffect(() => {
    if (!updatingId) return;

    const plugin = plugins.find(p => p.id === updatingId);

    if (plugin?.latestversion === plugin?.installedVersion) {
      setUpdatingId(null);
    }
  }, [plugins, updatingId]);

  return (
    <div className='p-6 bg-gray-50 min-h-screen'>
      <div className='flex justify-between items-center mb-6'>
        <h2 className='text-2xl font-bold text-gray-900'>Plugins</h2>
        <button onClick={handleScan} className='bg-primary text-white font-semibold px-4 py-2 rounded hover:bg-primary-dark transition-colors'>
          <RefreshCcw size={16} />
        </button>
      </div>
      <PluginTable data={plugins} handleUpdate={handleUpdate} updatingId={updatingId} />
    </div>
  )
}

export default PluginsPage;