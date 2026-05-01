import { AppDispatch } from "@/store";
import { HubConnection } from "@microsoft/signalr";
import { connectionStatusChanged, patchReceived } from "@/infrastructure/realtime/store/actions";
import { SignalRStatus } from "../types";

function registerSignalRHandlers(connection: HubConnection, dispatch: AppDispatch) {
  connection.on("ReceivePatch", (envelope) => {
    dispatch(patchReceived(envelope));
  });

  return () => {
    connection.off("ReceivePatch");
  };
}

function registerConnectionLifecycle(connection: HubConnection, dispatch: AppDispatch) {

  connection.onreconnecting((error) => {
    dispatch(connectionStatusChanged({
      status: SignalRStatus.Reconnecting,
      error: error?.message
    }));
  });

  connection.onreconnected((connectionId) => {
    dispatch(connectionStatusChanged({
      status: SignalRStatus.Connected
    }));
  });

  connection.onclose((error) => {
    dispatch(connectionStatusChanged({
      status: SignalRStatus.Disconnected,
      error: error?.message
    }));
  });
};

export {
  registerSignalRHandlers,
  registerConnectionLifecycle
}