import { EventEnvelope } from "@/shared/events/types";
import * as signalR from "@microsoft/signalr";
import { ConnectionStatus } from "./signalr.types";

interface CreateConnectionOptions {
  url: string;
  autoReconnect?: boolean;
}
export const createConnection = ({ url, autoReconnect = false }: CreateConnectionOptions) => {
  const builder = new signalR.HubConnectionBuilder()
    .withUrl(url)
    .configureLogging(signalR.LogLevel.Information);

  if (autoReconnect) {
    builder.withAutomaticReconnect([0, 2000, 10000]);
  }
  console.log(`create signalR connection to ${url}`);
  return builder.build();
}
  
export const createLifecycleEnvelope = (payload: ConnectionStatus):EventEnvelope => ({
  id: crypto.randomUUID(),
  type: 'connection.status.changed',
  sequence: -1,
  occurredAt: new Date().toISOString(),
  payload: payload
});