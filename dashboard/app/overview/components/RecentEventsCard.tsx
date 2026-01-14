'use client';

import Card from '@/components/Card';
import { DiagnosisRecord, RecordLevel } from '@/features/diagnosis/diagnosis.types';
import { formatDateTime } from '@/shared/utils/time';

interface RecentEventsCardProps {
  events: DiagnosisRecord[];
}

export default function RecentEventsCard({ events }: RecentEventsCardProps) {
  return (
    <Card className="h-56 flex flex-col">
      <p className="text-gray-900 font-semibold text-base mb-2">Recent Events</p>
      <ul className="text-sm space-y-1 overflow-y-auto">
        {events.map((e, idx) => (
          <li key={idx} className={getLevelClass(e.level)}>
            [{formatDateTime(e.timestamp)}] {e.level} {e.message}
          </li>
        ))}
      </ul>
    </Card>
  );
}

function getLevelClass(level: RecordLevel) {
  switch (level) {
    case 'Info':
      return "text-info";
    case 'Warn':
      return "text-warning font-medium";
    case 'Error':
      return "text-red-500 font-semibold";
    default:
      return "text-gray-600";
  }
}
