import { signalREventReceived } from "@/shared/events/signalr.actions";
import { AppDispatch } from "@/store";
import { HubConnection } from "@microsoft/signalr";

export function registerConnectionLifecycle(connection: HubConnection, dispatch: AppDispatch) {
  connection.onreconnecting(() => {
    dispatch(signalREventReceived({
      id: crypto.randomUUID(),
      type: 'SignalRReconnecting',
      sequence: -1,
      occurredAt: new Date().toISOString(),
      payload: null
    }));
  });
}