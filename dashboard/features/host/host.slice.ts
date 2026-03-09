import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { HostSnapshot, HostStatus } from "./host.types";

export interface HostState {
  status: HostStatus;
  startAt: string;
}

const initialState: HostState = {
  status: HostStatus.Idle,
  startAt: '',
}

export const hostSlice = createSlice({
  name: 'host',
  initialState,
  reducers: {
    snapshotInitialized(state, action: PayloadAction<HostSnapshot>) {
      state.status = action.payload.status;
      state.startAt = action.payload.startAt;
    }
  }
});

export const hostActions = hostSlice.actions;
export const hostReducer = hostSlice.reducer;