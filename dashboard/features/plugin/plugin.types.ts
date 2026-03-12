interface PluginEntry {
  id: string;
  name: string;
  version: string;
  status: PluginStatus;
}

interface PluginPackageEntry {
  id: string;
  version: string;
  downloadUrl: string;
  checksum: string;
}

interface PluginSet {
  entries: PluginEntry[],
  packages: PluginPackageEntry[]
}

type PluginStatus = 'loaded' | 'unloaded' | 'error';

export type {
  PluginEntry,
  PluginPackageEntry,
  PluginSet,
  PluginStatus
}