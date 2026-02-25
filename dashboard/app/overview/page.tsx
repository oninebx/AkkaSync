'use client';

import HostCard from "./components/HostCard/HostCard";
import RecentEventsCard from "./components/RecentEventsCard";
import { KpiBanner } from "./components/KpiBanner";
import { useSelector } from "react-redux";
import { selectConnectionStatus } from "@/infrastructure/signalr/connection.selectors";
import { HostStatus } from "@/features/host/host.types";
import { useSignalRInvoke } from "@/providers/SingalRProvider";
import PipelineTable from "./components/PipelineTable/PipelineTable";
import { selectHostPipelines } from "@/features/host/host.selectors";
import { selectScheduleSpecs, selectSecheduleJobs } from "@/features/scheduler/scheduler.selectors";
import { usePipelines } from "./components/PipelineTable/usePipelines";
import useKpis from "./components/KpiBanner/useKpis";
import { selectJournal } from "@/features/diagnosis/diagnosis.selectors";
interface PingResponse {
  value: string
}

export default function HomePage() {
  const connectionStatus = useSelector(selectConnectionStatus);
  const pipelines = useSelector(selectHostPipelines);
  const scheduleSpecs = useSelector(selectScheduleSpecs);
  const scheduleJobs = useSelector(selectSecheduleJobs);

  const pipelineData = usePipelines(pipelines, scheduleSpecs);
  const kpiData = useKpis(pipelines, scheduleJobs);

  const events = useSelector(selectJournal);
  const status = HostStatus.Idle;
  const startAt = new Date().toISOString();

  const { queryInvoke } = useSignalRInvoke<PingResponse>();
  
  // const handleClick = async () => {
  //   try{
  //     const data = await queryInvoke('QueryTest', { Value: 'ping' }, true);
  //     console.log(data);
  //   }catch(err){
  //     console.log(err);
  //   }
  // }
  // const handleReset = () => {
    
  // }
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