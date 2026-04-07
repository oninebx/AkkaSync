import { RootState } from "@/store";
import { workerAdapter } from "./worker.slice";

const workerState = (state: RootState) => state.workers;

const workerSelectors = workerAdapter.getSelectors(workerState);

export const selectWorkers = workerSelectors.selectAll;