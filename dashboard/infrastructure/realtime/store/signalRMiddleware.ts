import { Middleware } from "@reduxjs/toolkit";
import { connectionStatusChanged, patchReceived } from "./actions";
import { handlePatchMessage, handleStatusChange } from "./handlers";

const signalRMiddleware: Middleware = store => next => action => {
  try {
    console.log(action);
    if (patchReceived.match(action)) {
      const { sequence } = action.payload;

      console.group(`[SignalR realtime] Sequence: ${sequence}`);
      handlePatchMessage(store, action.payload);
      console.groupEnd();
    } else if (connectionStatusChanged.match(action)) {
      handleStatusChange(store, action.payload);
    }
  } catch(err){
    console.error('[SignalR Middleware Error]:', err);
  } finally {
    return next(action);
  }
}

export {
  signalRMiddleware
}