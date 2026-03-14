using AkkaSync.Infrastructure.SyncPlugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.Catalog
{
  public interface IPluginCatalog
  {
    Task<IReadOnlyList<PluginCatalogEntry>> GetAllAsync();
    Task AddAsync(PluginCatalogEntry entry);
    Task UpdateAsync(PluginCatalogEntry entry);
    Task RemoveAsync(string id, string version);
  }
}
