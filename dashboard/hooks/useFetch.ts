import { resolve } from "path";
import { useState, useEffect, useCallback, use } from "react";

type HttpMethod = "GET" | "POST" | "PUT" | "DELETE";

type UseFetchOptions = {
  method?: HttpMethod;
  body?: unknown;
  headers?: HeadersInit;
  skip?: boolean;
  retry?: number;
  retryDelay?: number;
};

const API_URL_BASE = process.env.REACT_APP_API_BASE_URL || '';
// function canonicalizeUrl(url: string) {
//   try{
//     return new URL(url).toString();
//   } catch {
//     return `${API_BASE_URL.replace(/\/$/, "")}/${url.replace(/^\//, "")}`;
//   }
// }

export interface ApiError{
  status: number;
  title: string;
  detail?: string;
  instance?: string;
}

export function useFetch<T = unknown>(
  endpoint: string,
  options?: UseFetchOptions
) {
  const { method = "GET", body, headers = {}, skip = false, retry = 0, retryDelay = 1000 } = options || {};

  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState<ApiError | null>(null);
  const [loading, setLoading] = useState<boolean>(!skip);
  const [attempt, setAttempt] = useState<number>(0);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);

    let currentAttempt = 0;

    while (currentAttempt <= retry) {
      try {
        const res = await fetch(resolve(API_URL_BASE, endpoint), {
          method,
          headers: {
            "Content-Type": "application/json",
            ...headers,
          },
          body: body ? JSON.stringify(body) : undefined,
        });

        if (!res.ok) {
          const problem = await res.json();
          throw problem;
          
        }

        const json = await res.json();
        setData(json);
        setLoading(false);
        return json;

      } catch (err: unknown) {
        currentAttempt++;
        if (currentAttempt > retry) {
          setError(err as ApiError);
          setLoading(false);
          return;
        }
        
        await new Promise(r => setTimeout(r, retryDelay));
      }
    }
  }, [endpoint, method, JSON.stringify(body), JSON.stringify(headers), retry, retryDelay]);

  useEffect(() => {
    if (!skip) {
      fetchData();
    }
  }, [fetchData, skip, attempt]);

  const triggerFetch = useCallback(() => {
    fetchData();
  }, [fetchData]);

  const refetch = () => setAttempt(prev => prev + 1);

  return { data, error, loading, refetch, triggerFetch };
}
