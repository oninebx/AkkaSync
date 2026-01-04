import { Column } from "@/components/DisplayTable";
import { selectHostPipelines } from "@/features/host/host.selectors";
import { cronToText, formatDuration, formatTimeMixed } from "@/shared/utils/time";
import { useSelector } from "react-redux";

interface OverviewPipeline {
  name: string;
  // status: "success" | "warning" | "error" | "running";
  // progress: number;
  schedule: string;
  duration: string;
  lastRun: string;
}

// const pipelinesData: OverviewPipeline[] = [
//   { name: "UserSync", schedule: "every 10 mins", activeRuns: '2 / 6', lastRun: 'Success(2m32s)' },
//   { name: "OrderSync", schedule: "Weekly", activeRuns: '1 / 5', lastRun: 'Failed'},
//   { name: "PaymentSync", schedule: "Daily at 2:00 pm", activeRuns: '2 / 3', lastRun: "Success(1m54s)" },
// ];

const usePipelines = () => {
  const pipelines = useSelector(selectHostPipelines);
  const data = pipelines.map(p => ({
    name: p.id,
    schedule: cronToText(p.schedule),
    duration: formatDuration(p.startedAt, p.finishedAt ?? new Date()),
    lastRun: formatTimeMixed(p.startedAt)
  } as OverviewPipeline));
  return data;
}

export type {
  OverviewPipeline
}

export {
  usePipelines
}


