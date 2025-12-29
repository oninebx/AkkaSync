'use client';

import Card from "@/components/Card";
import HostCard from "./components/HostCard/HostCard";
import RecentEventsCard from "./components/RecentEventsCard";
import { KpiBanner } from "./components/KpiBanner";
import { useSelector } from "react-redux";
import { selectConnectionStatus } from "@/infrastructure/signalr/connection.selectors";
import { selectEventsOrdered } from "@/features/recentevents/syncevents.selectors";
import { mapEnvelope } from "@/features/recentevents/syncevents.config";
import { HostStatus } from "@/features/host/host.types";
import { useSignalRInvoke, useSignalRQuery } from "@/providers/SingalRProvider";
import PipelineTable from "./components/PipelineTable/PipelineTable";
import SyncWorkerTable from "./components/SyncWorkerTable/SyncWorkerTable";
import { selectHostPipelines } from "@/features/host/host.selectors";
import useKpis from "./components/KpiBanner/useKpis";

// const events: EventItem[] = [
//   { time: "14:02:01", level: "INFO", message: "Pipeline UserSync started" },
//   { time: "14:02:02", level: "DEBUG", message: "Task FetchUsers completed (32ms)" },
//   { time: "14:02:03", level: "INFO", message: "Task SyncUsers completed (153ms)" },
// ];


interface PingResponse {
  value: string
}

export default function HomePage() {
  // const snapshot = useHostSnapshot();
  // const KpiData = useKpis(snapshot);
  // const { status, startAt } = snapshot;
  const connectionStatus = useSelector(selectConnectionStatus);
  const pipelines = useSelector(selectHostPipelines);

  const events = useSelector(selectEventsOrdered);
  const KpiData = [
    { id: 'running', title: "Running Pipelines", color: "#1F2937", value: '1' },
    { id: 'failed', title: "Failed (24h)", color: "#EF4444", value: '2' },
    { id: 'total', title: "Total Pipelines", color: "#1F2937", value: pipelines.length.toString() },
    { id: 'queued', title: "Queued Jobs", color: "#FBBF24", value: '4' },
  ];
  // const connectionStatus = 'connected';
  const status = HostStatus.Idle;
  const startAt = new Date().toISOString();

  // const {loading, data, error} = useSignalRQuery<PingResponse>('QueryTest', {Value: 'ping'}, true);
  // console.log(loading, data, error);
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
          <PipelineTable />
          <SyncWorkerTable />
          <div onClick={handleClick}>Ping Test</div>
        </div>
      </div>
    </>
  );
}