import { RootState } from "@/store";
import { scheduleJobAdapter } from "./scheduler.slice";

export const selectSchedulerState = (state: RootState) => state.scheduler;

export const schedulerSelectors = scheduleJobAdapter.getSelectors(
  selectSchedulerState
);

export const selectScheduleJobs = schedulerSelectors.selectAll;