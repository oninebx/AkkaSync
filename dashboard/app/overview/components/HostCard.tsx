import Card from "@/components/Card";
import { cn } from "@/lib/utils";

export type HostStatus = "online" | "offline" | "warning";

interface HostCardProps {
  name: string;
  status: HostStatus;
  lastHeartbeat?: string;
  className?: string;
}

export default function HostCard({ name, status, lastHeartbeat, className }: HostCardProps) {
  const statusColor = {
    online: "bg-success",
    offline: "bg-error",
    warning: "bg-warning",
  }[status];

  const statusText = {
    online: "Online",
    offline: "Offline",
    warning: "Unstable",
  }[status];

  return (
    <Card height="h-56" className={cn("flex flex-col justify-between", className)}>
      <div>
        <p className="text-gray-700 font-semibold">Host: {name}</p>

        <div className="flex items-center mt-2">
          <span className={cn("w-3 h-3 rounded-full mr-2", statusColor)}></span>
          <p className="text-gray-700 text-sm">{statusText}</p>
        </div>
      </div>

      {lastHeartbeat && (
        <p className="text-gray-500 mt-2 text-xs">
          Last Heartbeat: {lastHeartbeat}
        </p>
      )}
    </Card>
  );
}
