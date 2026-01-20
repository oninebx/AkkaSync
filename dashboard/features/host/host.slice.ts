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
    snapshotInitialized(state, action: PayloadAction<HostSnapshot>) {
      state.status = action.payload.status;
      state.startAt = action.payload.startAt;
      state.pipelines = action.payload.pipelines ?? [];
    },
    pipelineStarted(state, action: PayloadAction<PipelineSnapshot>) {
      const pipeline = state.pipelines.find(p => p.id === action.payload.id);
      if(pipeline){
        pipeline.startedAt = action.payload.startedAt;
      }
    },
    pipelineCompleted(state, action: PayloadAction<PipelineSnapshot>){
      const pipeline = state.pipelines.find(p => p.id === action.payload.id);
      if(pipeline){
        pipeline.finishedAt = action.payload.finishedAt;
      }
    }
  }
});

export const hostActions = hostSlice.actions;
export const hostReducer = hostSlice.reducer;