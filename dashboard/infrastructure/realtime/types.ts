enum ChangeOperation {
  Upsert = 0,
  Remove = 1,
  Replace = 2
}

interface ChangeSet {
  slice: string;
  operation: ChangeOperation;
  payload: any;
}

interface PatchEnvelope {
  id: string;
  sequence: number;
  payload: ChangeSet[];
  occurredAt: string;
}
type PatchReceivedPayload = PatchEnvelope;

const SignalRStatus = {
  Connecting: 'connecting',
  Connected: 'connected',
  Disconnected: 'disconnected',
  Reconnecting: 'reconnecting',
  Unavailable: 'unavailable'
} as const;

type SignalRStatus = typeof SignalRStatus[keyof typeof SignalRStatus];

interface SignalRStatusPayload {
  status: SignalRStatus;
  error?: string;
}

export type {
  ChangeSet,
  PatchReceivedPayload,
  SignalRStatusPayload,
}

export {
  ChangeOperation,
  SignalRStatus
}