import { PluginKind } from "@/contracts/plugin/types";
import { PluginStatus } from "@/features/plugin-artifact/plugin.types";

type PluginHealthStatus = 'loaded' | 'loadFailed' | 'notFound' | 'notDownloaded' | 'updateAvailable'

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