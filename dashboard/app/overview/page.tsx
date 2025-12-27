'use client';

import Card from "@/components/Card";
import HostCard from "./components/HostCard/HostCard";
import RecentEventsCard from "./components/RecentEventsCard";
import DisplayTable, { Column } from "@/components/DisplayTable";
import { cn } from "@/lib/utils";
import { useHostSnapshot } from "./hooks/useHostSnapshot";
import { KpiBanner } from "./components/KpiBanner";
import useKpis from "./components/KpiBanner/useKpis";
import { useSelector } from "react-redux";
import { selectConnectionStatus } from "@/infrastructure/signalr/connection.selectors";
import { selectEventsOrdered } from "@/features/recentevents/syncevents.selectors";
import { mapEnvelope } from "@/features/recentevents/syncevents.config";
import { HostStatus } from "@/features/host/host.types";
import { useSignalRInvoke, useSignalRQuery } from "@/providers/SingalRProvider";

// const events: EventItem[] = [
//   { time: "14:02:01", level: "INFO", message: "Pipeline UserSync started" },
//   { time: "14:02:02", level: "DEBUG", message: "Task FetchUsers completed (32ms)" },
//   { time: "14:02:03", level: "INFO", message: "Task SyncUsers completed (153ms)" },
// ];

const pipelinesData: Pipeline[] = [
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

interface PingResponse {
  value: string
}

export default function HomePage() {
  // const snapshot = useHostSnapshot();
  // const KpiData = useKpis(snapshot);
  // const { status, startAt } = snapshot;
  const connectionStatus = useSelector(selectConnectionStatus);
  const events = useSelector(selectEventsOrdered);
  const KpiData = [
    { id: 'running', title: "Running Pipelines", color: "#1F2937", value: '1' },
    { id: 'failed', title: "Failed (24h)", color: "#EF4444", value: '2' },
    { id: 'total', title: "Total Pipelines", color: "#1F2937", value: '3' },
    { id: 'queued', title: "Queued Jobs", color: "#FBBF24", value: '4' },
  ];
  // const connectionStatus = 'connected';
  const status = HostStatus.Idle;
  const startAt = new Date().toISOString();

  // const {loading, data, error} = useSignalRQuery<PingResponse>('QueryTest', {Value: 'ping'}, true);
  // console.log(loading, data, error);
  // console.log('-----');
  const { queryInvoke } = useSignalRInvoke<PingResponse>();
  
  const handleClick = async () => {
    try{
      const data = await queryInvoke('QueryTest', { Value: 'ping' }, true);
      console.log(data);
    }catch(err){
      console.log(err);
    }
  }
  return (
    <>
      <div className="min-h-screen px-4 py-6">
        <div className="max-w-7xl mx-auto space-y-6">
          <KpiBanner data={KpiData} />
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <HostCard 
              name="AkkaSync-Primary"
              connectionStatus={connectionStatus}
              status={status}
              startTime={startAt} />
            <RecentEventsCard events={events.map(e => mapEnvelope(e))}/>
          </div>
          <Card>
              <h2 className="text-lg font-semibold text-gray-700 mb-4">Pipelines Status</h2>
              <DisplayTable columns={columns} data={pipelinesData} />
          </Card>
          <div onClick={handleClick}>Ping Test</div>
        </div>
      </div>
    </>
  );
}