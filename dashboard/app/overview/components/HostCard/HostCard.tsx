import Card from "@/components/Card";
import { LabeledBadgeRow } from "@/app/overview/components/HostCard/LabeledBadgeRow";
import { cn } from "@/lib/utils";
import { HostStatus, StatusType } from "@/features/host/host.types";
import { CONNECTION_STATUS_COLORS, HOST_STATUS_CONFIG, } from "@/features/host/host.config";
import { ConnectionStatus } from "@/features/host/connection.types";

interface HostCardProps {
  name: string;
  connectionStatus: ConnectionStatus;
  status: HostStatus;
  startTime: string;
  className?: string;
}

export default function HostCard({ name, connectionStatus, status, startTime, className }: HostCardProps) {
  const colorConnection = CONNECTION_STATUS_COLORS[connectionStatus];
  const {color: colorHost, text: textHost} = HOST_STATUS_CONFIG[HostStatus[status].toLowerCase() as StatusType];
  return (
    <Card height="h-56" className={cn("flex flex-col justify-between relative overflow-hidden", className)}>
      <div
        className={cn('absolute top-0 left-0 right-0 h-1.5', colorConnection)}
        title={`SignalR: ${connectionStatus}`} />
      <div>
        <p className="text-gray-900 font-semibold text-base mb-2">Host: {name}</p>
        <LabeledBadgeRow label="State" color={colorHost} text={textHost} labelWidth="w-15" />
      </div>
      {startTime && (
        <p className="text-gray-500 mt-2 text-xs">
          Last Heartbeat: {startTime}
        </p>
      )}
    </Card>
  );
}
