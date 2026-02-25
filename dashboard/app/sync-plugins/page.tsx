'use client';
import React from 'react';
import PluginTable, { PluginListItem } from './components/PluginTable/PluginTable';
import { RefreshCcw } from 'lucide-react';

const mockPlugins: PluginListItem[] = [
  { id: 'source.plugina', name: 'Plugin A', type: 'source', version: '1.0.0', usedByCount: 2, status: 'loaded' },
  { id: 'transform.pluginb', name: 'Plugin B', type: 'transform', status: 'unloaded' },
  { id: 'sink.pluginc', name: 'Plugin C', type: 'sink', version: '1.2.0', usedByCount: 1, status: 'error' },
];

const PluginsPage = () => {
  const handleScan = () => {};
  return (
    <div className='p-6 bg-gray-50 min-h-screen'>
      <div className='flex justify-between items-center mb-6'>
        <h2 className='text-2xl font-bold text-gray-900'>Plugins</h2>
        <button onClick={handleScan} className='bg-primary text-white font-semibold px-4 py-2 rounded hover:bg-primary-dark transition-colors'>
          <RefreshCcw size={16} />
        </button>
      </div>
      <PluginTable data={mockPlugins} />
    </div>
  )
}

export default PluginsPage;