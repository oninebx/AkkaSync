import { createEntityAdapter, createSlice } from "@reduxjs/toolkit";
import { WorkerRecord } from "./worker.types";

export const workerAdapter = createEntityAdapter<WorkerRecord>();

export const workderSlice = createSlice({
  name: 'worker',
  initialState: workerAdapter.getInitialState(),
  reducers: {
    setWorkers: workerAdapter.setAll,
    workerAdded: workerAdapter.addOne,
    workerUpdated: workerAdapter.upsertOne,
    workerRemoved: workerAdapter.removeOne
  }
});

export const { setWorkers, workerAdded, workerUpdated, workerRemoved } = workderSlice.actions;
export const workerReducer = workderSlice.reducer;