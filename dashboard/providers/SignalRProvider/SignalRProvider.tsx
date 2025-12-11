'use client';
import { createContext, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { HostStatus, SignalREventMap, SignalRMethodMap, SignalRContextValue } from './types';
import { createConnection } from "./createConnection";
import { HandlerFunction } from "@/types/common";

export const SignalRContext = createContext<SignalRContextValue<SignalREventMap, SignalRMethodMap> | null>(null);

interface SignalRProviderProps {
  children: React.ReactNode;
  url: string;
  autoReconnect?: boolean;
}

export const SignalRProvider = ({ children, url, autoReconnect }: SignalRProviderProps) => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [status, setStatus] = useState<HostStatus>('connecting');

  const handlersRef = useRef<Map<string, Set<HandlerFunction>>>(new Map());

  useEffect(() => {
    const connection = createConnection({ url, autoReconnect });
    connectionRef.current = connection;

    const handlers = handlersRef.current;

    connection.start()
      .then(() => setStatus('online'))
      .catch(err => {
        console.error("SignalR Connection Error: ", err);
        setStatus('offline');
      });

    return () => {
      connection.stop();
      connectionRef.current = null;
      setStatus('offline');
      handlers.clear();
    }
  }, [url, autoReconnect]);

  const on = <K extends keyof SignalREventMap>(eventName: K, callback: SignalREventMap[K]) => {
    const eventKey = eventName as string;
    let set = handlersRef.current.get(eventKey);
    if(!set) {
      set = new Set<HandlerFunction>();
      handlersRef.current.set(eventKey, set);
    }
    if(!set.has(callback)) {
      connectionRef.current?.on(eventKey, callback);
      set.add(callback);
    }
    
  };
  const off = <K extends keyof SignalREventMap>(eventName: K, callback: SignalREventMap[K]) => {
    const eventKey = eventName as string;
    const set = handlersRef.current.get(eventKey);

    if (!set || !set.has(callback)) {
      console.warn(
        `SignalRProvider.off("${String(eventName)}") ignored: callback not found. ` +
        `You must pass the SAME function reference used in .on().`
      );
      return;
    }

    connectionRef.current?.off(eventKey, callback);
    set.delete(callback);
    if (set.size === 0) {
      handlersRef.current.delete(eventKey);
    }
  };
  const invoke = async <K extends keyof SignalRMethodMap>(methodName: K, ...args: Parameters<SignalRMethodMap[K]>): Promise<Awaited<ReturnType<SignalRMethodMap[K]>>> => {
    if (connectionRef.current) {
      return await connectionRef.current.invoke(methodName as string, ...args) as Awaited<ReturnType<SignalRMethodMap[K]>>;
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
