import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection | null = null;
export function getDashboardHub() {
  if(!connection){
    connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/hub/dashboard")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
  }
  return connection;
}