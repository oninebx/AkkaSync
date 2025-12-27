import { Action, createAction } from "@reduxjs/toolkit";
import { EventEnvelope } from "./types";

export const SIGNALR_ENVELOPE_RECEIVED = 'signalr/envelopeReceived';

export interface SignalREnvelopeReceivedAction extends Action<typeof SIGNALR_ENVELOPE_RECEIVED> {
  payload: EventEnvelope
}

export const signalREventReceived = createAction<EventEnvelope>(SIGNALR_ENVELOPE_RECEIVED);