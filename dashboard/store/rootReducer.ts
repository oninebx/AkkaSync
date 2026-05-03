import { pipelineConfigReducer, pipelineRuntimeReducer } from '@/features/pipelines';
import { pluginCacheReducer, pluginConfigReducer, pluginCentralReducer, pluginRuntimeReducer } from '@/features/plugins';
import { connectorConfigReducer } from '@/features/connector';

import { combineReducers } from "@reduxjs/toolkit";
import { schedulerReducer } from "@/features/scheduler/scheduler.slice";
import { diagnosisReducer } from "@/features/diagnosis/diagnosis.slice";
import { connectionReducer } from '@/infrastructure/realtime/store';

export const rootReducer = combineReducers({
  connection: connectionReducer,
  pipelineConfig: pipelineConfigReducer,
  pipelineRuntime: pipelineRuntimeReducer,
  pluginConfig: pluginConfigReducer,
  pluginCache: pluginCacheReducer,
  pluginCentral: pluginCentralReducer,
  pluginRuntime: pluginRuntimeReducer,
  connectorConfig: connectorConfigReducer,

  scheduler: schedulerReducer,
  diagnosis: diagnosisReducer,
});