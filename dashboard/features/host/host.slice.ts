import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ConnectionState, ConnectionStatus } from "./connection.types";
import { HostSnapshot } from "./host.types";

export interface HostState {
  snapshot: HostSnapshot | null;
  connection: ConnectionState;
}

const initialState: HostState = {
  snapshot: null,
  connection: {
    status: 'connecting',
    retryCount: 0
  }
}

export const hostSlice = createSlice({
  name: 'host',
  initialState,
  reducers: {
    snapshotUpdated(state, action: PayloadAction<HostSnapshot>){
      state.snapshot = action.payload;
    },
    connectionStatusChanged(state, action: PayloadAction<ConnectionStatus>){
      state.connection.status = action.payload;
    },
    connectionRetryIncremented(state) {
      state.connection.retryCount += 1;
    },
    connectionRetryReset(state) {
      state.connection.retryCount = 0;
    }
  }
});

export const hostActions = hostSlice.actions;
export const hostReducer = hostSlice.reducer;