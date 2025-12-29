export type ConnectionStatus = 'disconnected' | 'connecting' | 'connected' | 'unavailable';
export type ConnectionState = {
  status: ConnectionStatus;
  retryCount: number;
  lastError?: string;
}
export interface ConnectionStatusEvent {
  status: ConnectionStatus;
}