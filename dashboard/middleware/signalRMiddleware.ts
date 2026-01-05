import { EventEnvelope, PayloadEvent } from "@/shared/events/types";
import { signalREventReceived } from "@/shared/events/signalr.actions";
import { Middleware } from "@reduxjs/toolkit";
import { envelopeHandlers } from "./envelopeHandlers";

export const signalRMiddleware: Middleware = store => next => action => {
  
  if(signalREventReceived.match(action)) {
    console.log(action.payload);
    const envelope = action.payload as EventEnvelope<PayloadEvent<object>>;
    const handler = envelopeHandlers.get(envelope.type);
    if(handler) {
      handler(envelope.event, store.dispatch, store.getState);
      console.log(`${envelope.type}(${envelope.id}) is handled.`);
    }
  }
  return next(action);
}