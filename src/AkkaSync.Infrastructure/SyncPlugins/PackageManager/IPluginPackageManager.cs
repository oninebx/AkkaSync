using AkkaSync.Infrastructure.SyncPlugins.Models;

namespace AkkaSync.Infrastructure.SyncPlugins.PackageManager
{
  public interface IPluginPackageManager
  {
    Task<IReadOnlySet<PluginPackageEntry>> CheckoutVersions();
    Task<string> CheckoutPlugin(string url, string checksum);

  }
}
