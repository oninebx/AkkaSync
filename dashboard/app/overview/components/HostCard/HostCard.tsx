import Card from "@/components/Card";
import { cn } from "@/lib/utils";
import { SignalRStatus } from "@/infrastructure/realtime/types";

interface HostCardProps {
  name: string;
  status: SignalRStatus;
  startTime?: string;
  className?: string;
}

const CONNECTION_STATUS_COLORS: Record<SignalRStatus, string> = {
  connecting: 'bg-info',
  reconnecting: 'bg-info',
  connected: 'bg-success',
  unavailable: 'bg-error',
  disconnected: 'bg-gray-500'
}

export default function HostCard({ name, status, /*status,*/ startTime, className }: HostCardProps) {
  const colorConnection = CONNECTION_STATUS_COLORS[status];
  return (
    <Card height="h-56" className={cn("flex flex-col justify-between relative overflow-hidden", className)}>
      <div
        className={cn('absolute top-0 left-0 right-0 h-1.5', colorConnection)}
        title={`SignalR: ${status}`} />
      <div>
        <p className="text-gray-900 font-semibold text-base mb-2">Host: {name}</p>
        {/* <LabeledBadgeRow label="State" color={colorHost} text={textHost} labelWidth="w-15" /> */}
      </div>
      {startTime && (
        <p className="text-gray-500 mt-2 text-xs">
          Last Heartbeat: {startTime}
        </p>
      )}
    </Card>
  );
}
