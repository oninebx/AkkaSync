import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { HostSnapshot, HostStatus, PipelineSnapshot } from "./host.types";

export interface HostState {
  status: HostStatus;
  startAt: string;
  pipelines: PipelineSnapshot[];
}

const initialState: HostState = {
  status: HostStatus.Idle,
  startAt: '',
  pipelines: []
}

export const hostSlice = createSlice({
  name: 'host',
  initialState,
  reducers: {
    snapshotUpdated(state, action: PayloadAction<HostSnapshot>) {

      state.status = action.payload.status;
      state.startAt = action.payload.startAt;
      state.pipelines = action.payload.pipelines ?? [];
    }
  }
});

export const hostActions = hostSlice.actions;
export const hostReducer = hostSlice.reducer;