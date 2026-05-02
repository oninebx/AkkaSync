import { PluginHealthStatus, PluginKind } from "@/contracts/plugin/types";

interface PluginVM {
  id: string;
  name: string;
  kind: PluginKind;
  latestversion?: string;
  installedVersion?: string;
  usedByCount?: number;
  status: PluginHealthStatus;
  actions: string;
}

export type {
  PluginVM,
  PluginHealthStatus
}