'use client';
import { useState } from "react";
import { QueryEnvelope } from "./signalRProvider.types";
import { useSignalR } from "./useSignalR";

export const useSignalRInvoke = <TResult>() => {
  const { invoke } = useSignalR();

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const queryInvoke = async (
    method: string, 
    payload: Record<string, string> = {}, 
    returnImmediately: boolean = false): Promise<TResult> => {
      setLoading(true);
      setError(null);
      try {
        const query: QueryEnvelope = { method, payload, returnImmediately };
        const result = await invoke<TResult>('Query', query);
        return result;
      } catch(err){
        setError(err as Error);
        throw err;
      } finally{
        setLoading(false);
      }
    
    
  }
  return { queryInvoke, loading, error };
}