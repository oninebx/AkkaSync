import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { EventEnvelope } from "./syncevents.types";

interface EventsState {
  lastSeq: number;
  envelopes: Record<number, EventEnvelope>;
}

const initialState: EventsState = {
  lastSeq: 0,
  envelopes: {}
}

export const eventsSlice = createSlice({
  name: 'events',
  initialState,
  reducers: {
    eventsReceived(state, action: PayloadAction<EventEnvelope[]>){
      for(const envelope of action.payload) {
        if(state.envelopes[envelope.sequence]){
          continue;
        }
        state.envelopes[envelope.sequence] = envelope;
        if(envelope.sequence > state.lastSeq) {
          state.lastSeq = envelope.sequence;
        }
      }
    }
  }
});

export const eventsActions = eventsSlice.actions;
export const eventsReducer = eventsSlice.reducer;