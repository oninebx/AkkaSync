'use client';
import { createContext, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { HostSignalRContextValue, HostSignalREventMap, HostSignalRMethodMap, SignalRConnectionStatus } from './types';
import { createConnection } from "./createConnection";
import { HandlerFunction } from "@/types/common";

export const SignalRContext = createContext<HostSignalRContextValue | null>(null);

interface SignalRProviderProps {
  children: React.ReactNode;
  url: string;
  autoReconnect?: boolean;
}

export const SignalRProvider = ({ children, url, autoReconnect }: SignalRProviderProps) => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [status, setStatus] = useState<SignalRConnectionStatus>('connecting');

  const handlersRef = useRef<{[K in keyof HostSignalREventMap]?: Set<HostSignalREventMap[K]>}>({});

  useEffect(() => {
    const connection = createConnection({ url, autoReconnect });
    connectionRef.current = connection;

    const handlers = handlersRef.current;

    connection.start()
      .then(() => setStatus('connected'))
      .catch(err => {
        console.error("SignalR Connection Error: ", err);
        setStatus('unavailable');
      });

    return () => {
      connection.stop();
      connectionRef.current = null;
      setStatus('connecting');
      for (const key in handlers) {
        delete handlers[key as keyof HostSignalREventMap];
      }
    }
  }, [url, autoReconnect]);

  const on = <K extends keyof HostSignalREventMap>(eventName: K, callback: HostSignalREventMap[K]) => {
    let set = handlersRef.current[eventName];
    if(!set) {
      set = new Set<HandlerFunction>();
      handlersRef.current[eventName] = set;
    }
    if(!set.has(callback)) {
      connectionRef.current?.on(eventName, callback);
      set.add(callback);
    }
    
  };
  const off = <K extends keyof HostSignalREventMap>(eventName: K, callback: HostSignalREventMap[K]) => {
    const set = handlersRef.current[eventName];

    if (!set || !set.has(callback)) {
      console.warn(
        `SignalRProvider.off("${String(eventName)}") ignored: callback not found. ` +
        `You must pass the SAME function reference used in .on().`
      );
      return;
    }

    connectionRef.current?.off(eventName, callback);
    set.delete(callback);
    if (set.size === 0) {
      delete handlersRef.current[eventName];
    }
  };
  const invoke = async <K extends keyof HostSignalRMethodMap>(methodName: K, ...args: Parameters<HostSignalRMethodMap[K]>): Promise<Awaited<ReturnType<HostSignalRMethodMap[K]>>> => {
    const connection = connectionRef.current;
    if(!connection) {
      throw new Error('SignalR connection not initialized');
    }
    if(connection.state != signalR.HubConnectionState.Connected){
      throw new Error(
      `SignalR not connected. Current state: ${signalR.HubConnectionState[connection.state]}`
    );
    }
    return await connection.invoke(methodName as string, ...args) as Awaited<ReturnType<HostSignalRMethodMap[K]>>;
    
  }

  const value: HostSignalRContextValue = {
    status,
    on,
    off,
    invoke,
  };

  return (
    <SignalRContext.Provider value={value}>
      {children}
    </SignalRContext.Provider>
  );
};
