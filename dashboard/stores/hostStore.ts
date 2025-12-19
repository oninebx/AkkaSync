import { create } from 'zustand';
import type { HostSnapshot } from '@/types/host';

interface HostState {
  snapshot: HostSnapshot | null;
  loading: boolean;
  setSnapshot: (snapshot: HostSnapshot) => void;
  setLoading: (loading: boolean) => void;
}

export const useHostStore = create<HostState>(set => ({
  snapshot: null,
  loading: true,
  setSnapshot: snapshot => set({ snapshot, loading: false }),
  setLoading: (loading) => set({ loading })
}));

// import { Host } from "@/types/dashboard";
// import { create } from "zustand";

// interface HostState {
//   hosts: Host[];
//   addHost: (host: Host) => void;
//   updateHostStatus: (id: string, status: Host['status']) => void;
//   removeHost: (id: string) => void;
// }

// export const useHostStore = create<HostState>(set => ({
//   hosts: [],
//   addHost: (host: Host) => set(state => ({ hosts: [...state.hosts, host] })),
//   updateHostStatus: (id: string, status: Host['status']) => set(state => ({
//     hosts: state.hosts.map(host =>
//       host.id === id ? { ...host, status } : host
//     )
//   })),
//   removeHost: (id: string) => set(state => ({
//     hosts: state.hosts.filter(host => host.id !== id)
//   })),
// }));