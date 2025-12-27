import { EventEnvelope } from "@/shared/events/types";
import { signalREventReceived } from "@/shared/events/signalr.actions";
import { Middleware } from "@reduxjs/toolkit";
import { envelopeHandlers } from "./envelopeHandlers";

export const signalRMiddleware: Middleware = store => next => action => {
  if(signalREventReceived.match(action)) {
    const envelope = action.payload as EventEnvelope;
    const handler = envelopeHandlers.get(envelope.type);
    if(handler) {
      handler(envelope, store.dispatch, store.getState);
      console.log(`${envelope.type}(${envelope.id}) is handled.`);
    }
    console.log(envelope);
  }
  return next(action);
}