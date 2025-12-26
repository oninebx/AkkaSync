export interface EventEnvelope<T = unknown> {
  id: string;
  type: string;
  sequence: number;
  occurredAt: string;
  payload: T
}