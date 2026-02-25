
import { hostReducer } from "@/features/host/host.slice";
import { combineReducers } from "@reduxjs/toolkit";
import { connectionReducer } from "@/infrastructure/signalr/connection.slice";
import { schedulerReducer } from "@/features/scheduler/scheduler.slice";
import { diagnosisReducer } from "@/features/diagnosis/diagnosis.slice";
import { pluginHubReducers } from "@/features/plugin-hub/plugin-hub.slice";

export const rootReducer = combineReducers({
  connection: connectionReducer,
  host: hostReducer,
  scheduler: schedulerReducer,
  diagnosis: diagnosisReducer,
  pluginHub: pluginHubReducers
});