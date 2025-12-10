'use client';
import { createContext, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { HostStatus, SignalREventMap, SignalRMethodMap, SignalRContextValue } from './types';
import { createConnection } from "./createConnection";

export const SignalRContext = createContext<SignalRContextValue<SignalREventMap, SignalRMethodMap> | null>(null);

interface SignalRProviderProps {
  children: React.ReactNode;
  url: string;
  autoReconnect?: boolean;
}

export const SignalRProvider = ({ children, url, autoReconnect }: SignalRProviderProps) => {
  const connections = useRef<signalR.HubConnection | null>(null);
  const [status, setStatus] = useState<HostStatus>('connecting');

  useEffect(() => {
    const connection = createConnection({ url, autoReconnect });
    connections.current = connection;

    connection.start()
      .then(() => setStatus('online'))
      .catch(err => {
        console.error("SignalR Connection Error: ", err);
        setStatus('offline');
      });

    return () => {
      connection.stop();
      connections.current = null;
      setStatus('offline');
    }
  }, [url, autoReconnect]);

  const on = <K extends keyof SignalREventMap>(eventName: K, callback: SignalREventMap[K]) => {
    connections.current?.on(eventName as string, callback as (...args: unknown[]) => void);
  };
  const off = <K extends keyof SignalREventMap>(eventName: K, callback?: SignalREventMap[K]) => {
    connections.current?.off(eventName as string, callback as (...args: unknown[]) => void);
  };
  const invoke = async <K extends keyof SignalRMethodMap>(methodName: K, ...args: Parameters<SignalRMethodMap[K]>): Promise<Awaited<ReturnType<SignalRMethodMap[K]>>> => {
    if (connections.current) {
      return await connections.current.invoke(methodName as string, ...args) as Awaited<ReturnType<SignalRMethodMap[K]>>;
    }
    throw new Error("No connection available");
  }

  const value: SignalRContextValue<SignalREventMap, SignalRMethodMap> = {
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
