import { AppDispatch, RootState } from "@/store";

export interface EventEnvelope<T = unknown> {
  id: string;
  type: string;
  sequence: number;
  occurredAt: string;
  payload: T
}

export type EnvelopeHandler = (envelope: EventEnvelope, dispatch: AppDispatch, getState: () => RootState) => void;