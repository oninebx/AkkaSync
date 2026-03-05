using System;

namespace AkkaSync.Infrastructure.SyncPlugins.Models;

public sealed record PluginCacheEntry(string Id, string Version);
