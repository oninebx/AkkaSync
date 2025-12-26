interface QueryEnvelope {
  method: string;
  payload: Record<string, string>;
  returnImmediately: boolean;
}

export type {
  QueryEnvelope
}