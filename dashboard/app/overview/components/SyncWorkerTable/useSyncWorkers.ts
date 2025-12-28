import { BgColor } from "@/components/Badges"
import { OverviewSyncWorker, SyncWorkerStatus } from "./types"

const statusColor: Record<SyncWorkerStatus, BgColor>  = {
  success: "bg-success",
  warning: "bg-warning",
  error: "bg-error",
  running: "bg-info",
}

const useSyncWorkers = () => {
  const data: OverviewSyncWorker[] = [
    { name: 'UserSync', runId: 'a83f', status: 'running', stage: 'Fetch', progress: 63 }, 
    { name: 'UserSync', runId: 'a840', status: 'running', stage: 'Transform', progress: 18 },
    { name: 'OrderSync', runId: 'a841', status: 'success', stage: 'Load', progress: 100 },
    { name: 'PaymentSync', runId: 'a842', status: 'warning', stage: 'Write', progress: 85 },
    { name: 'PaymentSync', runId: 'a843', status: 'error', stage: 'Fetch', progress: 5  }
  ];
  return data;
}

export {
  useSyncWorkers,
  statusColor
}