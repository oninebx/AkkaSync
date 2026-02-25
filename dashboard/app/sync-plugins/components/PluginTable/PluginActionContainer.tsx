import { ActionButton } from '@/components/Buttons';
import { Trash2, Upload } from 'lucide-react';
import React from 'react';
import { PluginStatus } from './PluginTable';

type Props = {
  id: string;
  status: PluginStatus;
  handleUpload: (e: React.MouseEvent<HTMLButtonElement>) => void;
  handleRemove: (e: React.MouseEvent<HTMLButtonElement>) => void;
}

const PluginActionContainer = ({ id, status, handleUpload, handleRemove }: Props) => {
  return (
    <div className='flex gap-2'>
      <ActionButton color='blue' onClick={handleUpload} data-id={id}>
        <Upload size={16} />
      </ActionButton>
      <ActionButton color='red' onClick={handleRemove} disabled={status === 'unloaded'} data-id={id}>
        <Trash2 size={16} />
      </ActionButton>
    </div>
  )
}

export default PluginActionContainer