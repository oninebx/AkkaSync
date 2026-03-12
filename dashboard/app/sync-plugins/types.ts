import { PluginStatus } from "@/features/plugin/plugin.types";

interface PluginVM {
  id: string;
  name: string;
  type: 'source' | 'transform' | 'sink' | 'store';
  latestversion?: string;
  installedVersion?: string;
  usedByCount?: number;
  status: PluginStatus;
  url?: string;
  checksum?: string;
  pkgChecksum?: string;
  actions: string;
}

export type {
  PluginVM
}