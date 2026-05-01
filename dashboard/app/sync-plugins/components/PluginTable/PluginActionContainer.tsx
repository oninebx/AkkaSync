import { ActionButton } from '@/components/Buttons';
import { PluginStatus } from '@/features/plugin-artifact/plugin.types';
import { Trash2, DownloadCloud, CloudSync } from 'lucide-react';
import React from 'react';

type Props = {
  id: string;
  // latestVersion?: string;
  // installedVersion?: string;
  canUpgrade: boolean;
  disabled?: boolean;
  handleUpdate: (e: React.MouseEvent<HTMLButtonElement>) => void;
}

const PluginActionContainer = ({ id, /*latestVersion, installedVersion,*/ disabled, canUpgrade, handleUpdate }: Props) => {
  // const hasUpdate = installedVersion && latestVersion && installedVersion !== latestVersion;
  // const canLoad = !installedVersion && latestVersion;
  // const canUpgrade = hasUpdate;
  // const canUpdate = canLoad || canUpgrade;
  
  return (
    <div className='flex gap-2'>
      <ActionButton color={disabled ? 'gray' : 'blue'} disabled={disabled || !canUpgrade} onClick={handleUpdate} data-id={id}>
        <CloudSync size={16} />
      </ActionButton>
    </div>
  )
}

export default PluginActionContainer