import { selectConnectionStatus } from "@/features/host/connection.selectors";
import { HostSnapshot, HostStatus } from "@/features/host/host.types";
import { useHostSignalR } from "@/providers/SignalRProvider";
import { EventEnvelope } from "@/shared/events/EventEnvelope";


import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";

export const useHostSnapshot = () => {
  const { on, off } = useHostSignalR();
  const [snapshot, setSnapshot] = useState<HostSnapshot>({ status: HostStatus.Stopped, pipelinesTotal: 0, startAt: new Date().toUTCString()});
  const handleSnapshot = useCallback((envelope : EventEnvelope) => { 
    // setSnapshot(envelope.payload as HostSnapshot); 
    console.log(envelope);
  }, []);
  
  const status = useSelector(selectConnectionStatus);

  useEffect(() => {
    if(status === 'connected') {
      on('ReceiveDashboardEvent', handleSnapshot);
    }

    return () => {
      off('ReceiveDashboardEvent', handleSnapshot);
    }
  }, [status, off, on, handleSnapshot]);
 
  return {
    ...snapshot
  }
}