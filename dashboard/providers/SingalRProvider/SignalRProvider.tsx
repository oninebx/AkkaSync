'use client';

import { useDispatch } from "react-redux";
import * as signalR from "@microsoft/signalr";
import { createContext, ReactNode, useEffect, useRef } from "react";
import { createConnection } from "../../infrastructure/signalr/createConnection";
import { registerSignalRHandlers } from "@/infrastructure/signalr/registerHandlers";
import { registerConnectionLifecycle } from "@/infrastructure/signalr/retisterConnectionLifecycle";
import { QueryEnvelope } from "./types";

interface Props {
  url: string;
  autoReconnect: boolean,
  children: ReactNode
}



type EnvelopeInvokeMethod = <TPayload>(method: string, args?: QueryEnvelope) => Promise<TPayload>;

interface SignalRProviderValue {
  invoke: EnvelopeInvokeMethod;
}

export const SignalRContext = createContext<SignalRProviderValue | null>(null);

export const SignalRProvider = ({ children, url, autoReconnect = false }: Props) => {
  const dispatch = useDispatch();
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  
  useEffect(() => {

    const connection = createConnection({url, autoReconnect});
    connectionRef.current = connection;

    const unregister = registerSignalRHandlers(connection, dispatch);
    registerConnectionLifecycle(connection, dispatch);

    connection.start()
      .then(() => { console.log('signalR connected.'); })
      .catch(err => {console.error(err);});
    
    return () => {
      unregister();
      if(connection.state !== signalR.HubConnectionState.Disconnected) {
        connection.stop();
      }
      
      connectionRef.current = null;
    }
  }, [autoReconnect, dispatch, url]);

  const invoke = async <TPayload,>(method: string, args?: QueryEnvelope): Promise<TPayload> => {
    const connection = connectionRef.current;
    if(!connection) {
      throw new Error('SignalR connection not established');
    }
    return await connection.invoke(method, args);
  }

  return <SignalRContext.Provider value={{ invoke }}>{children}</SignalRContext.Provider>;
};

