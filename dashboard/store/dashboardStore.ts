import { create } from 'zustand';

export type DashboardEvent = {
  type: string;
  data: unknown;
}

type DashboardState = {
  events: DashboardEvent[];
  addEvent: (e: DashboardEvent) => void;
}

export const useDashboardStore = create<DashboardState>( set => ({
  events: [],
  addEvent: e => set(state => ({events: [...state.events, e]}))
}));