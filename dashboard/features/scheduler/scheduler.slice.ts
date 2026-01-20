import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { PipelineJob, PipelineSchedules } from "./scheduler.types";


interface SchedulerState {
  specs: Record<string, string>
  jobs: PipelineJob[]
}

const initialState: SchedulerState = {
  specs: {},
  jobs: []
}

export const schedulerSlice = createSlice({
  name: 'scheduler',
  initialState,
  reducers: {
    setSpecs(state, action: PayloadAction<Record<string, string>>){
      state.specs = action.payload;
    },
    setSchedules(state, action: PayloadAction<PipelineSchedules>) {
      state.jobs = action.payload.jobs;
      state.specs = action.payload.specs;
    },
    addJob(state, action: PayloadAction<PipelineJob>) {
      state.jobs.push(action.payload);
    },
    removeJob(state, action: PayloadAction<string>) {
      state.jobs = state.jobs.filter(job => job.name !== action.payload);
    }
  }
});

export const schedulerActions = schedulerSlice.actions;
export const schedulerReducer = schedulerSlice.reducer;