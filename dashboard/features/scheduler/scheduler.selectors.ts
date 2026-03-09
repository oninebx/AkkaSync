import { RootState } from "@/store";
import { scheduleJobAdapter } from "./scheduler.slice";

export const selectSchedulerState = (state: RootState) => state.scheduler;

export const schedulerSelectors = scheduleJobAdapter.getSelectors(
  selectSchedulerState
);

export const selectScheduleJobs = schedulerSelectors.selectAll;
// import { RootState } from "@/store";

// const selectScheduleSpecs = (state: RootState) => state.scheduler.specs;
// const selectSecheduleJobs = (state: RootState) => state.scheduler.jobs;

// export {
//   selectScheduleSpecs,
//   selectSecheduleJobs
// }