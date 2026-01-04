import { AppDispatch, RootState } from "@/store";

export interface PayloadEvent<TPayload> {
  type: string;
  payload: TPayload
}
export interface EventEnvelope<T extends object> {
  id: string;
  type: string;
  sequence: number;
  occurredAt: string;
  event: T
}

export type EnvelopeEventHandler = <T extends PayloadEvent<object>>(event: T, dispatch: AppDispatch, getState: () => RootState) => void;