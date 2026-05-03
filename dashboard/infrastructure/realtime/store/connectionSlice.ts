import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { SignalRStatus, SignalRStatusPayload } from "../types";
import { connectionStatusChanged } from "./actions";
import { RootState } from "@/store";

interface ConnectionState {
  status: SignalRStatus;
  lastConnectedAt: string | null;
  error: string | null;
}

const initialState: ConnectionState = {
  status: SignalRStatus.Disconnected,
  error: null,
  lastConnectedAt: null
}

const connectionSlice = createSlice({
  name: 'connection',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    }
  },
  extraReducers: builder => {
    builder.addCase(connectionStatusChanged, (state, action: PayloadAction<SignalRStatusPayload>) => {
      const { status, error } = action.payload;
      state.status = status;
      state.error = error ?? null;

      if (status === SignalRStatus.Connected) {
        state.lastConnectedAt = new Date().toISOString();
        state.error = null; 
      }
    })
  }
});

const selectors = {
  selectConnectionState: (state: RootState) => state.connection,
  selectIsConnected: (state: RootState) => state.connection.status === SignalRStatus.Connected
}

export const { clearError } = connectionSlice.actions;
export { 
  selectors as connectionSelectors
 };
export default connectionSlice.reducer;

