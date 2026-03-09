import { createEntityAdapter, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { PipelineJob, PipelineSchedules } from "./scheduler.types";

export const scheduleJobAdapter = createEntityAdapter<PipelineJob>();

type SchedulerState =   ReturnType<typeof scheduleJobAdapter.getInitialState<{ specs: Record<string, string> }>>

const initialState: SchedulerState = scheduleJobAdapter.getInitialState({ specs: {} })

export const schedulerSlice = createSlice({
  name: 'scheduler',
  initialState,
  reducers: {
    setSpecs(state, action: PayloadAction<Record<string, string>>){
      state.specs = action.payload;
    },
    setSchedules(state, action: PayloadAction<PipelineSchedules>) {
      const { jobs, specs } = action.payload;

      scheduleJobAdapter.setAll(state, jobs);
      state.specs = specs;
    },
    addJob(state, action: PayloadAction<PipelineJob>) {
      scheduleJobAdapter.addOne(state, action.payload);
    },
    removeJob(state, action: PayloadAction<string>) {
      scheduleJobAdapter.removeOne(state, action.payload);
    }
  }
});

export const schedulerActions = schedulerSlice.actions;
export const schedulerReducer = schedulerSlice.reducer;