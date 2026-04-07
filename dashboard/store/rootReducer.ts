
import { hostReducer } from "@/features/host/host.slice";
import { combineReducers } from "@reduxjs/toolkit";
import { connectionReducer } from "@/infrastructure/signalr/connection.slice";
import { schedulerReducer } from "@/features/scheduler/scheduler.slice";
import { diagnosisReducer } from "@/features/diagnosis/diagnosis.slice";
import { pluginReducers } from "@/features/plugin-artifact/plugin.slice";
import pipelineReducer from '@/features/pipeline/pipeline.slice';
import { workerReducer } from "@/features/worker/worker.slice";
import { pluginGraphReducer } from "@/features/plugin-graph/pluginGraph.slice";

export const rootReducer = combineReducers({
  pipeline: pipelineReducer,
  connection: connectionReducer,
  host: hostReducer,
  workers: workerReducer,
  scheduler: schedulerReducer,
  diagnosis: diagnosisReducer,
  plugin: pluginReducers,
  pluginGraph: pluginGraphReducer
});