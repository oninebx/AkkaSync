import { useHostStore } from "@/stores/hostStore";

export function useHostStatus() {
  const { snapshot, loading } = useHostStore();

  return {
    status: snapshot?.status ?? null,
    loading: loading
  }
}