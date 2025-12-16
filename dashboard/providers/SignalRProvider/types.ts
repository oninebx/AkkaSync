import { HostSnapshot } from "@/types/host";

export interface HostSignalREventMap {
  HostSnapshot: (payload: HostSnapshot) => void;
}

export type SignalRConnectionStatus = 'connecting' | 'connected' | 'unavailable';

export interface HostSignalRMethodMap {
  // [methodName: string]: (...args: unknown[]) => Promise<unknown>;
  GetHostSnapshot: () => Promise<HostSnapshot>;
}

export interface HostSignalRContextValue {
  status: SignalRConnectionStatus;
  on: <K extends keyof HostSignalREventMap>(eventName: K, callback: HostSignalREventMap[K]) => void;
  off: <K extends keyof HostSignalREventMap>(eventName: K, callback: HostSignalREventMap[K]) => void;
  invoke: <K extends keyof HostSignalRMethodMap>(methodName: K, ...args: Parameters<HostSignalRMethodMap[K]>) => Promise<Awaited<ReturnType<HostSignalRMethodMap[K]>>>;
}