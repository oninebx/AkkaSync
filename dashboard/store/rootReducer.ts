
import { eventsReducer } from "@/features/recentevents/syncevents.slice";
import { hostReducer } from "@/features/host/host.slice";
import { combineReducers } from "@reduxjs/toolkit";
import { connectionReducer } from "@/infrastructure/signalr/connection.slice";

export const rootReducer = combineReducers({
  connection: connectionReducer,
  host: hostReducer,
  events: eventsReducer
});