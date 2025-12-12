// components/host/RecentEventsCard.tsx
'use client';

import Card from '@/components/Card';

export interface EventItem {
  time: string;
  level: "INFO" | "DEBUG" | "ERROR";
  message: string;
}

interface RecentEventsCardProps {
  events: EventItem[];
}

export default function RecentEventsCard({ events }: RecentEventsCardProps) {
  return (
    <Card className="h-56 flex flex-col">
      <p className="text-gray-700 font-semibold mb-2">Recent Events</p>
      <ul className="text-sm space-y-1 overflow-y-auto">
        {events.map((e, idx) => (
          <li key={idx} className={getLevelClass(e.level)}>
            [{e.time}] {e.level} {e.message}
          </li>
        ))}
      </ul>
    </Card>
  );
}

// 根据日志等级动态返回颜色
function getLevelClass(level: string) {
  switch (level) {
    case "INFO":
      return "text-info";
    case "DEBUG":
      return "text-gray-500";
    case "ERROR":
      return "text-red-500 font-semibold";
    default:
      return "text-gray-600";
  }
}
