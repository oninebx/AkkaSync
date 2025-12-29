import { Action, createAction } from "@reduxjs/toolkit";
import { EventEnvelope } from "./types";

export const SIGNALR_ENVELOPE_RECEIVED = 'signalr/envelopeReceived';

export interface SignalREnvelopeReceivedAction<T extends object> extends Action<typeof SIGNALR_ENVELOPE_RECEIVED> {
  payload: EventEnvelope<T>
}

export const signalREventReceived = createAction<EventEnvelope<object>>(SIGNALR_ENVELOPE_RECEIVED);