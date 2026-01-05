import { createSlice } from "@reduxjs/toolkit";

interface SchedulerState {
  schedules: Record<string, string[]>
}

const initialState: SchedulerState = {
  schedules: {}
}

export const schedulerSlice = createSlice({
  name: 'scheduler',
  initialState,
  reducers: {

  }
});