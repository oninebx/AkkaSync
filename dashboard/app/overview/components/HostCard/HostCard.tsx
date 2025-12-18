import Card from "@/components/Card";
import { StatusRow } from "@/components/StatusRow";
import { cn } from "@/lib/utils";
import { SignalRConnectionStatus } from "@/providers/SignalRProvider";
import { useHostSnapshot } from "@/app/overview/hooks/useHostSnapshot";
import { HostStatus, StatusType } from "@/types/host";



interface HostCardProps {
  name: string;
  connectionStatus: SignalRConnectionStatus;
  status: HostStatus;
  startTime: string;
  className?: string;
}

const STATUS_COLORS: Record<SignalRConnectionStatus, string> = {
  connecting: 'bg-info',
  connected: 'bg-success',
  unavailable: 'bg-error'
}

export default function HostCard({ name, connectionStatus, status, startTime, className }: HostCardProps) {
  const color = STATUS_COLORS[connectionStatus];
  const hostState = HostStatus[status].toLowerCase() as StatusType;
  return (
    <Card height="h-56" className={cn("flex flex-col justify-between relative overflow-hidden", className)}>
      <div
        className={cn('absolute top-0 left-0 right-0 h-1.5', color)}
        title={`SignalR: ${connectionStatus}`} />
      <div>
        <p className="text-gray-900 font-semibold text-base mb-2">Host: {name}</p>
        <StatusRow label="State" status={hostState} labelWidth="w-15" />
      </div>
      {startTime && (
        <p className="text-gray-500 mt-2 text-xs">
          Last Heartbeat: {startTime}
        </p>
      )}
    </Card>
  );
}
