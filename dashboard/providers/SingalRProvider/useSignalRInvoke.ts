'use client';
import { useContext, useState } from "react";
import { SignalRContext } from "./SignalRProvider";
import { QueryEnvelope } from "./types";

export const useSignalRInvoke = <TResult>() => {
  const context = useContext(SignalRContext);
  if(!context || !context.invoke){
    throw new Error('SignalR connection not established');
  }
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
        const result = await context.invoke<TResult>('Query', query);
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