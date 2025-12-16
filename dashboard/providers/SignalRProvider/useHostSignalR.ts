import { useContext } from 'react';
import { HostSignalRContextValue } from './types';
import { SignalRContext } from './HostSignalRProvider';

export const useHostSignalR = () => {
  const context = useContext<HostSignalRContextValue | null>(SignalRContext);
  if (!context) {
    throw new Error("useHostSignalR must be used within a SignalRProvider");
  }
  return { ...context };
};