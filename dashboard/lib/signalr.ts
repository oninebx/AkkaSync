import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection | null = null;
const URL_HUB_DASHBOARD = process.env.NEXT_PUBLIC_SIGNALR_HUB_DASHBOARD || "https://localhost:5001/hub/dashboard";
export function getDashboardHub() {
  if(!connection){
    connection = new signalR.HubConnectionBuilder()
    .withUrl(URL_HUB_DASHBOARD)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
  }
  return connection;
}