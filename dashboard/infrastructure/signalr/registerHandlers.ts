import { EventEnvelope } from "@/shared/events/types";
import { signalREventReceived } from "@/shared/events/signalr.actions";
import { AppDispatch } from "@/store";
import { HubConnection } from "@microsoft/signalr";

export function registerSignalRHandlers(connection: HubConnection, dispatch: AppDispatch) {
  const onEvent = <T extends object>(envelope: EventEnvelope<T>) => {
    console.log('--------', envelope);
    dispatch(signalREventReceived(envelope));
  }
  connection.on('ReceiveDashboardEvent', onEvent);
  return () => {
    connection.off('ReceiveDashboardEvent', onEvent);
  }
}