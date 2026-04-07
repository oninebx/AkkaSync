import { PluginType } from "@/contracts/plugin/types";
import { PluginStatus } from "@/features/plugin-artifact/plugin.types";

interface PluginVM {
  id: string;
  name: string;
  type: PluginType;
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