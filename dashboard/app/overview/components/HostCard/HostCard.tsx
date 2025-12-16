import Card from "@/components/Card";
import { StatusRow } from "@/components/StatusRow";
import { cn } from "@/lib/utils";
import { SignalRConnectionStatus } from "@/providers/SignalRProvider";
import { useHostSnapshot } from "@/app/overview/hooks/useSnapshot";
import { HostStatus, StatusType } from "@/types/host";



interface HostCardProps {
  name: string;
  className?: string;
}

const STATUS_COLORS: Record<SignalRConnectionStatus, string> = {
  connecting: 'bg-info',
  connected: 'bg-success',
  unavailable: 'bg-error'
}

export default function HostCard({ name, className }: HostCardProps) {
  const { connectionStatus, status, timestamp } = useHostSnapshot();
  const color = STATUS_COLORS[connectionStatus];
  const hostStatus = HostStatus[status].toLowerCase() as StatusType;
  return (
    <Card height="h-56" className={cn("flex flex-col justify-between relative overflow-hidden", className)}>
      <div
        className={cn('absolute top-0 left-0 right-0 h-1.5', color)}
        title={`SignalR: ${connectionStatus}`} />
      <div>
        <p className="text-gray-900 font-semibold text-base mb-2">Host: {name}</p>
        <StatusRow label="Host" status={ hostStatus } labelWidth="w-15" />
        {/* <StatusRow label="Engine" status={engineStatus} labelWidth="w-15" /> */}
      </div>
      {timestamp && (
        <p className="text-gray-500 mt-2 text-xs">
          Last Heartbeat: {timestamp}
        </p>
      )}
    </Card>
  );
}
