import { resolveUrl } from "@/utils/url";
import * as signalR from "@microsoft/signalr";
import { useCallback, useEffect, useRef, useState } from "react";

interface SignalREventMap {
  [eventName: string] : (...args: unknown[]) => void;
}

const SIGNALR_URL_BASE = process.env.NEXT_PUBLIC_SIGNALR_BASE_URL;

const useSignalR = <TEvents extends SignalREventMap>(url: string, options: signalR.IHttpConnectionOptions = {}, autoReconnect = true) =>{
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [connected, setConnected] = useState(false);
  const on = useCallback(<K extends keyof TEvents>(eventName: K, callback: TEvents[K]) => {
    if(connectionRef.current){
      connectionRef.current.on(eventName as string, callback as (...args: unknown[]) => void);
    }
  }, []);
  const off = useCallback(<K extends keyof TEvents>(eventName: K, callback?: TEvents[K]) => {
    if(connectionRef.current){
      connectionRef.current.off(eventName as string, callback as (...args: unknown[]) => void);
    }
  }, []);

  const invoke = useCallback(async<K extends keyof TEvents>(methodName: K, ...args: unknown[]) => {
    if(connectionRef.current){
      return await connectionRef.current.invoke(methodName as string, ...args);
    }
  }, []);
  useEffect(() => {
    const builder = new signalR.HubConnectionBuilder()
      .withUrl(resolveUrl(SIGNALR_URL_BASE, url), options ?? {})
      .configureLogging(signalR.LogLevel.Information);
    if (autoReconnect) {
      builder.withAutomaticReconnect();
    }
    const connection = builder.build();
    connectionRef.current = connection;
    connection.start()
      .then(() => {
        console.log("SignalR Connected");
        setConnected(true);
      })
      .catch(err => console.error("SignalR Connection Error: ", err));
      return () => {
        connection.stop();
        connectionRef.current = null;
        setConnected(false);
      };
  }, [url, options, autoReconnect]);
  return { connected, on, off, invoke };
}

export default useSignalR;