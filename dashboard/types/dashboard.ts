import { HostStatus } from "@/providers/SignalRProvider/SignalRProvider";

interface Host {
  id: string;
  name: string;
  signalRHubUrl: string;
  status: HostStatus;
}

export type {
   Host 
};