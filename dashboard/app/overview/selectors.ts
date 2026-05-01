import { RootState } from "@/store";
import { createSelector } from "@reduxjs/toolkit";

import { selectScheduleJobs } from "@/features/scheduler/scheduler.selectors";
import { KpiVM, PipelineVM } from "./types";
import { pipelineConfigSelectors, pipelineRuntimeSelectors } from "@/features/pipelines";


const KPI_TEMPLATES = [
  { id: "running", title: "Running Pipelines", color: "#1F2937" },
  { id: "failed", title: "Failed (24h)", color: "#EF4444" },
  { id: "total", title: "Total Pipelines", color: "#1F2937" },
  { id: "queued", title: "Queued Jobs", color: "#FBBF24" }
];

export const selectKpiData = createSelector(
  [pipelineConfigSelectors.selectEntities, selectScheduleJobs],
  (pipelines, jobs): KpiVM[] => {

    const total = pipelines.length;

    //  calculating by pipeline runtime status
    const running = 0;

    const failed = 0;

    const queued = jobs.length;

    return KPI_TEMPLATES.map(template => {
      switch (template.id) {
        case "total":
          return { ...template, value: String(total) };

        case "running":
          return { ...template, value: String(running) };

        case "failed":
          return { ...template, value: String(failed) };

        case "queued":
          return { ...template, value: String(queued) };

        default:
          return { ...template, value: "0" };
      }
    });
  }
);