import { Column } from "@/components/DisplayTable";

interface OverviewPipeline {
  name: string;
  // status: "success" | "warning" | "error" | "running";
  // progress: number;
  schedule: string;
  activeRuns: string;
  lastRun: string;
}

const pipelinesData: OverviewPipeline[] = [
  { name: "UserSync", schedule: "every 10 mins", activeRuns: '2 / 6', lastRun: 'Success(2m32s)' },
  { name: "OrderSync", schedule: "Weekly", activeRuns: '1 / 5', lastRun: 'Failed'},
  { name: "PaymentSync", schedule: "Daily at 2:00 pm", activeRuns: '2 / 3', lastRun: "Success(1m54s)" },
];

const usePipelines = () => {
  return pipelinesData;
}

export type {
  OverviewPipeline
}

export {
  usePipelines
}


