'use client';

import { useDispatch } from "react-redux";
import * as signalR from "@microsoft/signalr";
import { createContext, ReactNode, useEffect, useRef } from "react";
import { QueryEnvelope } from "./signalRProvider.types";
import { registerConnectionLifecycle, registerSignalRHandlers } from "./signalRProvider.handlers";
import { connectionStatusChanged } from "../store/actions";
import { SignalRStatus } from "../types";

interface Props {
  url: string;
  autoReconnect: boolean,
  children: ReactNode
}

type EnvelopeInvokeMethod = <TPayload>(method: string, args?: QueryEnvelope) => Promise<TPayload>;

interface SignalRProviderValue {
  invoke: EnvelopeInvokeMethod;
}

interface CreateConnectionOptions {
  url: string;
  autoReconnect?: boolean;
}
const createConnection = ({ url, autoReconnect = false }: CreateConnectionOptions) => {
  const builder = new signalR.HubConnectionBuilder()
    .withUrl(url)
    .configureLogging(signalR.LogLevel.Information);

  if (autoReconnect) {
    builder.withAutomaticReconnect([0, 2000, 10000]);
  }
  console.log(`create signalR connection to ${url}`);
  return builder.build();
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
      .then(() => dispatch(connectionStatusChanged({
        status: SignalRStatus.Connected
      })))
      .catch(err => { 
        dispatch(connectionStatusChanged({ 
          status: SignalRStatus.Unavailable, 
          error: err instanceof Error ? err.message : String(err)
        }));
      });
    
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
    if(!connection || connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('SignalR connection is not established or not in Connected state');
    }
    return await connection.invoke(method, args);
  }

  return <SignalRContext.Provider value={{ invoke }}>{children}</SignalRContext.Provider>;
};

