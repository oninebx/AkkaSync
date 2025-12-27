import { signalREventReceived } from "@/shared/events/signalr.actions";
import { AppDispatch } from "@/store";
import { HubConnection } from "@microsoft/signalr";
import { createLifecycleEnvelope } from "./connection.utils";

export function registerConnectionLifecycle(connection: HubConnection, dispatch: AppDispatch) {
  connection.onreconnecting(() => dispatch(signalREventReceived(createLifecycleEnvelope('connecting'))));
  connection.onreconnected(() => dispatch(signalREventReceived(createLifecycleEnvelope('connected'))));
  connection.onclose(() => dispatch(signalREventReceived(createLifecycleEnvelope('disconnected'))));
}