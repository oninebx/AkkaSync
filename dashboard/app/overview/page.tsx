'use client';

import HostCard from "./components/HostCard/HostCard";
import RecentEventsCard from "./components/RecentEventsCard";
import { KpiBanner } from "./components/KpiBanner";
import { useSelector } from "react-redux";
import PipelineTable from "./components/PipelineTable";

import { selectJournal } from "@/features/diagnosis/diagnosis.selectors";
import { selectKpiData } from "./selectors";
import { connectionSelectors } from "@/infrastructure/realtime/store";
import { pipelineRuntimeSelectors } from "@/features/pipelines/pipelineRuntime.slice";
interface PingResponse {
  value: string
}

export default function HomePage() {
  const connectionStatus = useSelector(connectionSelectors.selectConnectionStatus);

  
  const pipelines = useSelector(pipelineRuntimeSelectors.selectAll);
 
  const kpiData = useSelector(selectKpiData);

  const events = useSelector(selectJournal);
  // const startAt = new Date().toISOString();

  return (
    <>
      <div className="min-h-screen px-4 py-6">
        <div className="max-w-7xl mx-auto space-y-6">
          <KpiBanner data={kpiData}/>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <HostCard 
              name="AkkaSync-Primary"
              status={connectionStatus}
              /*status={status}*/ />
            <RecentEventsCard events={events}/>
          </div>
          <PipelineTable data={pipelines} />
        </div>
      </div>
    </>
  );
}