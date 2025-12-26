export type ConnectionStatus = 'disconnected' | 'connecting' | 'connected' | 'unavailable';
export type ConnectionState = {
  status: ConnectionStatus;
  retryCount: number;
}