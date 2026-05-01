import { connectorConfigSelectors } from "@/features/connector";
import { RootState } from "@/store";
import { createSelector } from "@reduxjs/toolkit";

const selectConnectors = connectorConfigSelectors.selectAll;

const selectPipelineTopogy = createSelector([
  selectConnectors
],(connectors) => {
  
});

// import { createSelector } from '@reduxjs/toolkit';
// import {
//   selectBasePipelineEntities,
//   selectPipelineDefinitionMap,
//   selectPipelineRunEntities
// } from '@/features/pipeline/pipeline.selectors';
// import { DEFAULT_LIVE } from '@/features/pipeline/pipeline.defaults';

// export const selectClusterPipelines = createSelector(
//   selectBasePipelineEntities,
//   selectPipelineDefinitionMap,
//   selectPipelineRunEntities,
//   (entities, definition, run) =>
//     Object.keys(definition).map((id) => {
//       const base = entities[id] ?? { id };
//       const def = definition[id];
//       const l = run[id];

//       // 👉 从 definition 提取 source / target
//       const source =
//         def?.source?.name ?? '-';

//       const target =
//         def?.target?.name ?? '-';

//       const { id: _, ...liveRest } = l ?? {};
//       return {
//         id: base.id,
//         name: base.name,

//         source,
//         target,

//         ...DEFAULT_LIVE,
//         ...liveRest,

//         // 👉 时间字段统一处理
//         // startedAt: l?.currentRunTime
//         //   ? Date.now() - l.currentRunTime
//         //   : undefined,

//         // nextRunAt: l?.nextRunTime ?? undefined
//       };
//     })
// );