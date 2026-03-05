using System;

namespace AkkaSync.Infrastructure.SyncPlugins.Models;

public sealed record PluginPackageEntry(string Id, string Version, string Url, string Checksum);
