import { Column } from "@/components/DisplayTable";
import { selectHostPipelines } from "@/features/host/host.selectors";
import { cronToNext, cronToText, formatDuration, formatTimeMixed } from "@/shared/utils/time";
import { useSelector } from "react-redux";

interface OverviewPipeline {
  name: string;
  // status: "success" | "warning" | "error" | "running";
  // progress: number;
  schedule: string;
  duration: string;
  lastRun: string;
  nextRun: string;
}

const usePipelines = () => {
  const pipelines = useSelector(selectHostPipelines);
  const data = pipelines.map(p => ({
    name: p.id,
    schedule: cronToText(p.schedule),
    duration: formatDuration(p.startedAt, p.finishedAt ?? new Date()),
    lastRun: formatTimeMixed(p.startedAt),
    nextRun: formatTimeMixed(cronToNext(p.schedule))
  } as OverviewPipeline));
  return data;
}

export type {
  OverviewPipeline
}

export {
  usePipelines
}


