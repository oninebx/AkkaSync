import { cn } from '@/lib/utils';
import { hasNewVersion } from '@/shared/utils/version';
import React from 'react'

interface Props {
  name: string;
  installedVersion?: string;
  latestVersion?: string;
}

const NameCell = ({name, installedVersion, latestVersion}: Props) => {

  const getState = () => {
    if (!installedVersion && latestVersion) return "upgrade";
    if (installedVersion && latestVersion && hasNewVersion(installedVersion, latestVersion)) return 'upgrade'; 
    if (!installedVersion && !latestVersion) return "unloaded";
    return "loaded";
  };

  const state = getState();

  const CONFIG = {
    loaded: {
      style: "text-green-600",
      icon: "●",
    },
    unloaded: {
      style: "text-yellow-600",
      icon: "○",
    },
    upgrade: {
      style: "text-blue-600 font-semibold",
      icon: "⬆",
    },
  };

 

  const { style, icon } = CONFIG[state];

  return (
    <div className="flex items-center gap-2">
      <span className={style} title={state}>
        {icon}
      </span>
      <span className='font-medium text-gray-600'>
        {name}
      </span>
    </div>
  );
}

export default NameCell