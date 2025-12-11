import { HandlerFunction } from "@/types/common";

export type HostStatus = 'online' | 'offline' | 'syncing' | 'connecting';

export interface SignalREventMap {
  [eventName: string]: HandlerFunction;
}

export interface SignalRMethodMap {
  [methodName: string]: (...args: unknown[]) => Promise<unknown>;
}

export interface SignalRContextValue<TEvents extends SignalREventMap, TMethods extends SignalRMethodMap> {
  status: HostStatus;
  on: <K extends keyof TEvents>(eventName: K, callback: TEvents[K]) => void;
  off: <K extends keyof TEvents>(eventName: K, callback: TEvents[K]) => void;
  invoke: <K extends keyof TMethods>(methodName: K, ...args: Parameters<TMethods[K]>) => Promise<Awaited<ReturnType<TMethods[K]>>>;
}