'use client';

import { useDispatch } from "react-redux";
import * as signalR from "@microsoft/signalr";
import { createContext, ReactNode, useCallback, useEffect, useRef } from "react";
import { QueryEnvelope } from "./signalRProvider.types";
import { registerConnectionLifecycle, registerSignalRHandlers } from "./signalRProvider.handlers";
import { connectionStatusChanged } from "../store/actions";
import { SignalRStatus } from "../types";
import { AsyncFunction } from "@/types";
import { SignalRStatusContainer } from "./SignalRNotifyBar";

interface Props {
  url: string;
  autoReconnect: boolean,
  children: ReactNode
}

type EnvelopeInvokeMethod = <TPayload>(method: string, args?: QueryEnvelope) => Promise<TPayload>;

interface SignalRProviderValue {
  invoke: EnvelopeInvokeMethod;
  reconnect: AsyncFunction<[], void>;
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
  
  const startConnection = useCallback(async () => {
    if(!connectionRef.current){
      return;
    }
    if(connectionRef.current.state !== signalR.HubConnectionState.Disconnected) {
      return;
    }
    
    try {
      await connectionRef.current.start();
      dispatch(connectionStatusChanged({ status: SignalRStatus.Connected }));
    }catch(err){
      dispatch(connectionStatusChanged({
        status: SignalRStatus.Disconnected,
        error: String(err)
      }));
    }

  }, [dispatch]);

  useEffect(() => {

    const connection = createConnection({url, autoReconnect});
    connectionRef.current = connection;

    const unregister = registerSignalRHandlers(connection, dispatch);
    registerConnectionLifecycle(connection, dispatch);

    startConnection();

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

  return (
    <SignalRContext.Provider value={{ invoke, reconnect: startConnection }}>
      <SignalRStatusContainer />
      {children}
    </SignalRContext.Provider>);
};

