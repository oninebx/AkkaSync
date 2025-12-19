import { useHostSignalR } from "@/providers/SignalRProvider";
import { HostSnapshot, HostStatus } from "@/types/host";

import { useCallback, useEffect, useRef, useState } from "react";

export const useHostSnapshot = () => {
  const {status, on, off } = useHostSignalR();
  const [snapshot, setSnapshot] = useState<HostSnapshot>({ status: HostStatus.Stopped, pipelinesTotal: 0, startAt: new Date().toUTCString()});
  const handleSnapshot = useCallback((payload : HostSnapshot) => { 
    setSnapshot(payload); 
    console.log('on: HostSnapshot---->', payload);
  }, []);

  useEffect(() => {
    if(status != 'connected') {
      return;
    }
    on('HostSnapshot', handleSnapshot);

    return () => {
      off('HostSnapshot', handleSnapshot);
    }
  }, [status, off, on, handleSnapshot]);
 
  return {
    connectionStatus: status, 
    ...snapshot
  }
}