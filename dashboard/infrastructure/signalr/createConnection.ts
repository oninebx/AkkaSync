import * as signalR from "@microsoft/signalr";

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