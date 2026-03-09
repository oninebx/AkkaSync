import { RootState } from "@/store";
import { createSelector } from "@reduxjs/toolkit";
import { cronToNext, cronToText, formatDuration, formatTimeMixed } from "@/shared/utils/time";
import { pipelineSelectors } from "@/features/pipeline/pipeline.slice";
import { selectPipelines } from "@/features/pipeline/pipeline.selectors";
import { selectScheduleJobs } from "@/features/scheduler/scheduler.selectors";
import { KpiVM, PipelineVM } from "./types";

export const selectPipelineData = createSelector(
  [
    pipelineSelectors.selectAll,
    (state: RootState) => state.scheduler.specs
  ],
  (pipelines, schedules): PipelineVM[] =>
    pipelines.map(p => {
      const pipelineSchedule = schedules?.[p.schedule];
      return {
        name: p.id,
        schedule: pipelineSchedule ? cronToText(pipelineSchedule) : '-',
        duration: p.startAt
          ? formatDuration(p.startAt, p.finishAt ?? new Date()) ?? "-"
          : "-",
        lastRun: p.startAt ? formatTimeMixed(p.startAt) : "-",
        nextRun: pipelineSchedule ? formatTimeMixed(cronToNext(pipelineSchedule)) : '-'
      };
    })
);

const KPI_TEMPLATES = [
  { id: "running", title: "Running Pipelines", color: "#1F2937" },
  { id: "failed", title: "Failed (24h)", color: "#EF4444" },
  { id: "total", title: "Total Pipelines", color: "#1F2937" },
  { id: "queued", title: "Queued Jobs", color: "#FBBF24" }
];

export const selectKpiData = createSelector(
  [selectPipelines, selectScheduleJobs],
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