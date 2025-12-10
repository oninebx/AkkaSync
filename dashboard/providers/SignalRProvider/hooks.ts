import { useContext } from 'react';
import { SignalRContextValue, SignalREventMap, SignalRMethodMap } from './types';
import { SignalRContext } from './SignalRProvider';

export const useHostStatus = () => {
  const context = useContext<SignalRContextValue<SignalREventMap, SignalRMethodMap> | null>(SignalRContext);
  if (!context) {
    throw new Error("useHostStatus must be used within a SignalRProvider");
  }
  return context.status;
};