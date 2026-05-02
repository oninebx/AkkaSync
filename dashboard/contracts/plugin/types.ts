type PluginKind = 'source' | 'transform' | 'sink' | 'unknown';
type PluginStatus = 'running' | 'idle' | 'succeeded' | 'failed';
type PluginHealthStatus = 'loaded' | 'loadFailed' | 'notFound' | 'notDownloaded' | 'updateAvailable';

export type {
  PluginKind,
  PluginStatus,
  PluginHealthStatus
}