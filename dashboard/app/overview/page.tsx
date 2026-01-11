'use client';

import Card from "@/components/Card";
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
  const scheduleSpecs = useSelector(selectScheduleSpecs);
  const scheduleJobs = useSelector(selectSecheduleJobs);

  const pipelineData = usePipelines(pipelines, scheduleSpecs);
  const kpiData = useKpis(pipelines, scheduleJobs);

  const events = useSelector(selectJournal);
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
          <div onClick={handleClick}>Ping Test</div>
        </div>
      </div>
    </>
  );
}