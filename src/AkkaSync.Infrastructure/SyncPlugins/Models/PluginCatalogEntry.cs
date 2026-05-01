using System;
namespace AkkaSync.Infrastructure.SyncPlugins.Models
{
  public sealed record PluginCatalogEntry(string Id, string Version, string Checksum, bool PendingDelete);
}
