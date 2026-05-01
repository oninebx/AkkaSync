import { createAction } from "@reduxjs/toolkit";
import { PatchReceivedPayload, SignalRStatusPayload } from "../types";

const SignalRActions = {
  patchReceived: createAction('signalr/patchReceived', (payload: PatchReceivedPayload) => ({
    payload,
    meta: {
      receiveAt: new Date().toISOString(),
    }
  })),
  connectionStatusChanged: createAction<SignalRStatusPayload>('signalr/statusChanged'),
};

export const { patchReceived, connectionStatusChanged } = SignalRActions;
