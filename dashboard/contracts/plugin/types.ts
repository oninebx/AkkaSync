type PluginKind = 'source' | 'transform' | 'sink' | 'unknown';
type PluginStatus = 'running' | 'idle' | 'succeeded' | 'failed';

export type {
  PluginKind,
  PluginStatus
}