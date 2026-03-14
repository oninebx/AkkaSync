using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.Models
{
  public sealed record PluginCatalogEntry(string Id, string Version, bool PendingDelete);
}
