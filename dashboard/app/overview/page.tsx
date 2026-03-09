'use client';

import HostCard from "./components/HostCard/HostCard";
import RecentEventsCard from "./components/RecentEventsCard";
import { KpiBanner } from "./components/KpiBanner";
import { useSelector } from "react-redux";
import { selectConnectionStatus } from "@/infrastructure/signalr/connection.selectors";
import { HostStatus } from "@/features/host/host.types";
import PipelineTable from "./components/PipelineTable/PipelineTable";

import { selectJournal } from "@/features/diagnosis/diagnosis.selectors";
import { selectKpiData, selectPipelineData } from "./selectors";
interface PingResponse {
  value: string
}

export default function HomePage() {
  const connectionStatus = useSelector(selectConnectionStatus);

  const pipelineData = useSelector(selectPipelineData);
 
  const kpiData = useSelector(selectKpiData);

  const events = useSelector(selectJournal);
  const status = HostStatus.Idle;
  const startAt = new Date().toISOString();

  return (
    <>
      <div className="min-h-screen px-4 py-6">
        <div className="max-w-7xl mx-auto space-y-6">
          <KpiBanner data={kpiData}/>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <HostCard 
              name="AkkaSync-Primary"
              connectionStatus={connectionStatus}
              status={status}
              startTime={startAt} />
            <RecentEventsCard events={events}/>
          </div>
          <PipelineTable data={pipelineData} />
        </div>
      </div>
    </>
  );
}