using System;

namespace AkkaSync.Host.Application.Swapping;

public sealed record PluginPackageInfo(string Id, string Version, string DownloadUrl);
