import { createSlice } from "@reduxjs/toolkit";
import { ConnectionState } from "./signalr.types";

const initialState: ConnectionState = {
  status: 'connecting',
  retryCount: 0
}

export const connectionSlice = createSlice({
  name: 'connection',
  initialState,
  reducers: {
    connectionStatusChanged(state, action){
      state.status = action.payload
    },
    connectionRetryIncremented(state) {
      state.retryCount += 1;
    },
    connectionRetryReset(state) {
      state.retryCount = 0;
    }
  }
});

export const connectionActions = connectionSlice.actions;
export const connectionReducer = connectionSlice.reducer;