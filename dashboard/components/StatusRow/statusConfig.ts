import { StatusType } from "@/types/host";

export const STATUS_CONFIG: Record<StatusType, { color: string; text: string }> = {
  syncing: { color: 'bg-info', text: 'Syncing' },
  idle: { color: 'bg-gray-200', text: 'Idle' },
  degraded: { color: 'bg-warning', text: 'Degraded'},
  stopped: {color: 'bg-gray-500', text: 'Stopped'},
};