import { PipelineSnapshot } from "@/features/host/host.types";
import { cronToNext, cronToText, formatDuration, formatTimeMixed } from "@/shared/utils/time";
interface OverviewPipeline {
  name: string;
  // status: "success" | "warning" | "error" | "running";
  // progress: number;
  schedule: string;
  duration: string;
  lastRun: string;
  nextRun: string;
}

const usePipelines = (pipelines: PipelineSnapshot[], schedules: Record<string, string>) => {
  const data = pipelines?.map(p => {
    const schedule = schedules[p.id] ?? '0 1 * * *';
    return {
      name: p.id,
      schedule: cronToText(schedule),
      duration: formatDuration(p.startedAt, p.finishedAt ?? new Date()),
      lastRun: formatTimeMixed(p.startedAt),
      nextRun: formatTimeMixed(cronToNext(schedule))
    } as unknown as OverviewPipeline
  });
  return data;
}

export type {
  OverviewPipeline
}

export {
  usePipelines
}


