'use client';
import { startTransition, useContext, useEffect, useState } from "react";
import { SignalRContext } from "./SignalRProvider";
import { QueryEnvelope } from "./types";

const buildQueryKey = ({method, payload, returnImmediately}: QueryEnvelope) => {
  const sortedPayload = Object.keys(payload)
    .sort()
    .map(k => `${k}:${payload[k]}`)
    .join('|');
  return `${method}|${returnImmediately}|${sortedPayload}`;
}

export const useSignalRQuery = <TResult,>(method: string, payload: Record<string, string>, returnImmediately: boolean = false) => {
  const context = useContext(SignalRContext);
  if(!context || !context.invoke){
    throw new Error('SignalR connection not established');
  }

  const [data, setData] = useState<TResult>();
  const [error, setError] = useState<Error>();
  const [loading, setLoading] = useState(true);
  
  const queryKey = buildQueryKey({method, payload, returnImmediately});
  useEffect(() => {
    let cancelled = false;
    startTransition(() => {
      setLoading(true);
      setError(undefined);
  });
    const query = { method, payload, returnImmediately } as QueryEnvelope;
    context.invoke<TResult>('Query', query)
      .then(result => !cancelled && setData(result))
      .catch(err => !cancelled && setError(err))
      .finally(() => !cancelled && setLoading(false));

    return () => {
      cancelled = true;
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [context, queryKey]);
  return { data, error, loading }
}