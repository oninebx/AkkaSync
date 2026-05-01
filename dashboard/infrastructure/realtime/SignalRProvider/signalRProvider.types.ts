interface QueryEnvelope {
  method: string;
  payload: Record<string, string>;
  returnImmediately: boolean;
}

interface QueryResponse {
  success: boolean,
  message: string
}

export type {
  QueryEnvelope,
  QueryResponse
}