import { RootState } from "@/store";

const selectScheduleSpecs = (state: RootState) => state.scheduler.specs;
const selectSecheduleJobs = (state: RootState) => state.scheduler.jobs;

export {
  selectScheduleSpecs,
  selectSecheduleJobs
}