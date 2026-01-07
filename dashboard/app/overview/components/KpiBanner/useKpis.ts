import { useMemo } from "react";
import { Kpi } from "./KpiCard";
import { PipelineSnapshot } from "@/features/host/host.types";
import { PipelineJob } from "@/features/scheduler/scheduler.types";

const KPI_TEMPLATES = [
  { id: 'running', title: "Running Pipelines", color: "#1F2937" },
  { id: 'failed', title: "Failed (24h)", color: "#EF4444" },
  { id: 'total', title: "Total Pipelines", color: "#1F2937" },
  { id: 'queued', title: "Queued Jobs", color: "#FBBF24" },
];

const useKpis = (pipelines: PipelineSnapshot[], jobs: PipelineJob[]): Kpi[] => {
  return useMemo(() => {
    // const pipelines = pipelines ?? [];
    const total = pipelines.length;
    // const running = pipelines.length;
    const running = 0;
    const failed = 0;
    const queued = jobs.length;

    return KPI_TEMPLATES.map(template => {
      switch(template.id){
        case 'total':
          return {...template, value: String(total)};
        case 'running':
          return { ...template, value: String(running) };
        case 'failed':
          return { ...template, value: String(failed) };
        case 'queued':
          return { ...template, value: String(queued) };

        default:
          return { ...template, value: '0' };
      }
    });
  }, [pipelines, jobs]);
};

export default useKpis;