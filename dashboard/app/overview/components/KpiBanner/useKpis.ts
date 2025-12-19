import { HostSnapshot } from "@/types/host";
import { useMemo } from "react";
import { Kpi } from "./KpiCard";

const KPI_TEMPLATES = [
  { id: 'running', title: "Running Pipelines", color: "#1F2937" },
  { id: 'failed', title: "Failed (24h)", color: "#EF4444" },
  { id: 'total', title: "Total Pipelines", color: "#1F2937" },
  { id: 'queued', title: "Queued Jobs", color: "#FBBF24" },
];

const useKpis = (snapshot: HostSnapshot): Kpi[] => {
  return useMemo(() => {
    const pipelines = snapshot?.pipelines ?? [];
    const total = snapshot.pipelinesTotal;
    const running = pipelines.length;
    const failed = 0;
    const queued = 0;

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
  }, [snapshot])
};

export default useKpis;