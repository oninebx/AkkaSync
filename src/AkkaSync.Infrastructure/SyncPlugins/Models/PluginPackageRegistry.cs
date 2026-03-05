using System;

namespace AkkaSync.Infrastructure.SyncPlugins.Models;

public sealed record PluginPackageRegistry(string ReleaseTag, HashSet<PluginPackageEntry> Plugins);
