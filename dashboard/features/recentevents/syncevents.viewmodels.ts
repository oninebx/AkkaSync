export interface EventItem {
  time: string;
  level: EventLevel;
  message: string;
}
export type EventLevel = 'INFO' | 'DEBUG' | 'ERROR';