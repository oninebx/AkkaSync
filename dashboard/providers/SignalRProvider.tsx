'use client';
import { createContext, useContext, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";

type HostStatus = 'online' | 'offline' | 'syncing' | 'connecting';

interface SignalREventMap {
  [eventName: string] : (...args: unknown[]) => void;
}

interface SignalRMethodMap {
  [methodName: string] : (...args: unknown[]) => Promise<unknown>;
}

interface SignalRContextValue<TEvents extends SignalREventMap, TMethods extends SignalRMethodMap> {
  status: HostStatus;
  on: <K extends keyof TEvents>(eventName: K, callback: TEvents[K]) => void;
  off: <K extends keyof TEvents>(eventName: K, callback?: TEvents[K]) => void;
  invoke: <K extends keyof TMethods>(methodName: K, ...args: Parameters<TMethods[K]>) => Promise<Awaited<ReturnType<TMethods[K]>>>;
}

const SignalRContext = createContext<SignalRContextValue<SignalREventMap, SignalRMethodMap> | null>(null);

interface SignalRProviderProps {
  children: React.ReactNode;
  url: string;
  autoReconnect?: boolean;
}

const SignalRProvider = ({children, url, autoReconnect}: SignalRProviderProps) => {
  const connections = useRef<signalR.HubConnection | null>(null);
  const [status, setStatus] = useState<HostStatus>('connecting');
  useEffect(() => {
    const builder = new signalR.HubConnectionBuilder()
      .withUrl(url)
      .configureLogging(signalR.LogLevel.Information);
    if (autoReconnect) {
      builder.withAutomaticReconnect([0, 2000, 10000]);
    }
    const connection = builder.build();
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
  const invoke = async<K extends keyof SignalRMethodMap>(methodName: K, ...args: Parameters<SignalRMethodMap[K]>): Promise<Awaited<ReturnType<SignalRMethodMap[K]>>> => {
    if(connections.current){
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
      { children }
    </SignalRContext.Provider>
  )
}

const useHostStatus = () =>{
  
  const context = useContext(SignalRContext);
  if(!context){
    throw new Error("useHostStatus must be used within a SignalRProvider");
  }
  return context.status;
}


export type {
  HostStatus,
  SignalREventMap,
}

export {
  useHostStatus,
}

export default SignalRProvider;