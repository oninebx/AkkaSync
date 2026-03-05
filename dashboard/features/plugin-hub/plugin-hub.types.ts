interface PluginEntry {
  name: string;
  version: string;
  state: 'Loaded' | 'Unloaded';
}

interface PluginPackageEntry {
  name: string;
  version: string;
  downloadUrl: string;
}

interface PluginSet {
  entries: PluginEntry[],
  packages: PluginPackageEntry[]
}

type PluginStatus = 'loaded' | 'unloaded' | 'error';

interface PluginListItem {
  id: string;
  name: string;
  type: 'source' | 'transform' | 'sink' | 'store';
  version?: string;
  usedByCount?: number;
  status: PluginStatus;
  actions?: string[];
}

export type {
  PluginEntry,
  PluginPackageEntry,
  PluginSet,
  PluginStatus,
  PluginListItem
}