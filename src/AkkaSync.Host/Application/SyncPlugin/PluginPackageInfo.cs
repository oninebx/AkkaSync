using System;

namespace AkkaSync.Host.Application.Plugin;

public sealed record PluginPackageInfo(string Id, string Version, string DownloadUrl, string Checksum);
