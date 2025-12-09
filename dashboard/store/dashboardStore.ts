import { create } from 'zustand';

export type DashboardEvent = {
  type: string;
  data: unknown;
}

type DashboardState = {
  hosts: { id: string; name: string; status: 'online' | 'offline' | 'syncing' }[];
  events: DashboardEvent[];
  addEvent: (e: DashboardEvent) => void;
}

export const useDashboardStore = create<DashboardState>( set => ({
  hosts: [],
  events: [],
  addEvent: e => set(state => ({events: [...state.events, e]}))
}));