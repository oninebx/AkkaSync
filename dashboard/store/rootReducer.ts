
import { eventsReducer } from "@/features/syncevents/syncevents.slice";
import { hostReducer } from "@/features/host/host.slice";
import { combineReducers } from "@reduxjs/toolkit";

export const rootReducer = combineReducers({
  host: hostReducer,
  events: eventsReducer
});