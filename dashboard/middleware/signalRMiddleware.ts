import { EventEnvelope } from "@/shared/events/EventEnvelope";
import { signalREventReceived } from "@/shared/events/signalr.actions";
import { RootState } from "@/store";
import { Action, AnyAction, Middleware, UnknownAction } from "@reduxjs/toolkit";

export const signalRMiddleware: Middleware = store => next => action => {
  if(signalREventReceived.match(action)) {
    const envelope = action.payload;
    console.log(envelope);
  }
  return next(action);
}