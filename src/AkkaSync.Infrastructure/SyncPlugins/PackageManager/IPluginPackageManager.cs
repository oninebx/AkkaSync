using AkkaSync.Infrastructure.Messaging.Models;
using AkkaSync.Infrastructure.SyncPlugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.PackageManager
{
  public interface IPluginPackageManager
  {
    Task<IReadOnlySet<PluginPackageEntry>> CheckoutVersions();
    Task<string> CheckoutPlugin(string url, string checksum);
  }
}
