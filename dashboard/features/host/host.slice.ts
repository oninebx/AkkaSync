import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { HostSnapshot } from "./host.types";

export interface HostState {
  snapshot: HostSnapshot | null;
}

const initialState: HostState = {
  snapshot: null,
}

export const hostSlice = createSlice({
  name: 'host',
  initialState,
  reducers: {
    snapshotUpdated(state, action: PayloadAction<HostSnapshot>){
      state.snapshot = action.payload;
    }
  }
});

export const hostActions = hostSlice.actions;
export const hostReducer = hostSlice.reducer;