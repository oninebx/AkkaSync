import { ConnectionStatus } from "../../infrastructure/signalr/signalr.types";
import { StatusType } from "./host.types";

export const HOST_STATUS_CONFIG: Record<StatusType, { color: string; text: string }> = {
  syncing: { color: 'bg-info', text: 'Syncing' },
  idle: { color: 'bg-gray-200', text: 'Idle' },
  degraded: { color: 'bg-warning', text: 'Degraded'},
  stopped: {color: 'bg-gray-500', text: 'Stopped'},
};

export const CONNECTION_STATUS_COLORS: Record<ConnectionStatus, string> = {
  connecting: 'bg-info',
  connected: 'bg-success',
  unavailable: 'bg-error',
  disconnected: 'bg-gray-500'
}