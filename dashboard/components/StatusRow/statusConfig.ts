import { StatusType } from "@/types/host";

export const STATUS_CONFIG: Record<StatusType, { color: string; text: string }> = {
  online: { color: 'bg-success', text: 'Online' },
  offline: { color: 'bg-error', text: 'Offline' },
  paused: { color: 'bg-warning', text: 'Paused' },
  syncing: { color: 'bg-info', text: 'Syncing' },
  failed: { color: 'bg-error', text: 'Failed' },
  stopped: { color: 'bg-gray-500', text: 'Stopped' },
  idle: { color: 'bg-gray-200', text: 'Idle' },
  starting: { color: 'bg-warning', text: 'Starting' },
};