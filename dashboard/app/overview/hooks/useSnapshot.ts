import { useHostSignalR } from "@/providers/SignalRProvider";
import { HostSnapshot, HostStatus } from "@/types/host";

import { useEffect, useState } from "react";

export const useHostSnapshot = () => {
  const {status, on, off, invoke} = useHostSignalR();
  const [snapshot, setSnapshot] = useState<HostSnapshot>({ status: HostStatus.Offline, timestamp: new Date().toUTCString()});
  const handleSnapshot = (payload : HostSnapshot) => { setSnapshot(payload); console.log(payload)};
  
  useEffect(() => {
    on('HostSnapshot', handleSnapshot);

    return () => {
      off('HostSnapshot', handleSnapshot);
    }
  }, [on, off]);
  
  useEffect(() => {
    if(status != 'connected'){
      return;
    }
    let cancelled = false;
    const initHost = async () => {
      try{
        const payload = await invoke('GetHostSnapshot');
        if(!cancelled) {
          setSnapshot(payload);
        }
      }catch(ex){
        console.log(ex);
      }
    };
    initHost();
    return () => { 
      cancelled = true; 
    }
  }, [status, invoke]);
  return {
    connectionStatus: status, 
    ...snapshot
  }
}