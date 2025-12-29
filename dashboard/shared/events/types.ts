import { AppDispatch, RootState } from "@/store";

export interface EventEnvelope<T extends object> {
  id: string;
  type: string;
  sequence: number;
  occurredAt: string;
  event: T
}

export type EnvelopeEventHandler = <T extends object>(event: T, dispatch: AppDispatch, getState: () => RootState) => void;