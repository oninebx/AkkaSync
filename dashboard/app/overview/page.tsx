'use client';

import Card from "@/components/Card";
import KpiCard from "./components/KpiCard";
import HostCard from "./components/HostCard/HostCard";
import RecentEventsCard, { EventItem } from "./components/RecentEventsCard";
import DisplayTable, { Column } from "@/components/DisplayTable";
import { cn } from "@/lib/utils";
import { useHostSnapshot } from "./hooks/useSnapshot";

const kpis = [
  { title: "Running Pipelines", value: "2", color: "#1F2937" },
  { title: "Failed (24h)", value: "0", color: "#EF4444" },
  { title: "Total Pipelines", value: "8", color: "#1F2937" },
  { title: "Queued Jobs", value: "1", color: "#FBBF24" },
];

const events: EventItem[] = [
  { time: "14:02:01", level: "INFO", message: "Pipeline UserSync started" },
  { time: "14:02:02", level: "DEBUG", message: "Task FetchUsers completed (32ms)" },
  { time: "14:02:03", level: "INFO", message: "Task SyncUsers completed (153ms)" },
];

const pipelines: Pipeline[] = [
  { name: "UserSync", status: "running", progress: 60, start: "14:00", duration: "2m" },
  { name: "OrderSync", status: "success", progress: 100, start: "13:50", duration: "5m" },
  { name: "PaymentSync", status: "error", progress: 45, start: "13:55", duration: "3m" },
];

interface Pipeline {
  name: string;
  status: "success" | "warning" | "error" | "running";
  progress: number;
  start: string;
  duration: string;
}

const columns: Column<Pipeline>[] = [
  { key: "name", header: "Pipeline" },
  {
    key: "status",
    header: "Status",
    render: (p) => {
      const statusColor = {
        success: "bg-success text-white",
        warning: "bg-warning text-white",
        error: "bg-error text-white",
        running: "bg-info text-white",
      }[p.status];
      return (
        <span className={cn("inline-block px-2 py-1 rounded", statusColor)}>
          {p.status}
        </span>
      );
    },
  },
  {
    key: "progress",
    header: "Progress",
    render: (p) => (
      <div className="w-full bg-gray-200 rounded-full h-2">
        <div
          className={cn(
            "h-2 rounded-full",
            p.status === "success"
              ? "bg-success"
              : p.status === "warning"
              ? "bg-warning"
              : p.status === "error"
              ? "bg-error"
              : "bg-info"
          )}
          style={{ width: `${p.progress}%` }}
        />
      </div>
    ),
    },
  { key: "start", header: "Start Time" },
  { key: "duration", header: "Duration" },
];

export default function HomePage() {

  return (
    <>
      <div className="min-h-screen px-4 py-6">
        <div className="max-w-7xl mx-auto space-y-6">
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6">
            { kpis.map((kpi) => 
                <KpiCard key={kpi.title} title={kpi.title} value={kpi.value} color={kpi.color} />)
            }
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <HostCard name="AkkaSync-Primary" />
            <RecentEventsCard events={events}/>
          </div>
          <Card>
              <h2 className="text-lg font-semibold text-gray-700 mb-4">Pipelines Status</h2>
              <DisplayTable columns={columns} data={pipelines} />
          </Card>
        </div>
      </div>
    </>
  );
}