interface PluginEntry {
  name: string;
  version: string;
  state: 'Loaded' | 'Unloaded';
}

interface PluginSet {
  entries: PluginEntry[]
}

export type {
  PluginEntry,
  PluginSet
}