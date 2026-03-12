
import { hostReducer } from "@/features/host/host.slice";
import { combineReducers } from "@reduxjs/toolkit";
import { connectionReducer } from "@/infrastructure/signalr/connection.slice";
import { schedulerReducer } from "@/features/scheduler/scheduler.slice";
import { diagnosisReducer } from "@/features/diagnosis/diagnosis.slice";
import { pluginReducers } from "@/features/plugin/plugin.slice";
import pipelineReducer from '@/features/pipeline/pipeline.slice';

export const rootReducer = combineReducers({
  pipeline: pipelineReducer,
  connection: connectionReducer,
  host: hostReducer,
  scheduler: schedulerReducer,
  diagnosis: diagnosisReducer,
  plugin: pluginReducers
});