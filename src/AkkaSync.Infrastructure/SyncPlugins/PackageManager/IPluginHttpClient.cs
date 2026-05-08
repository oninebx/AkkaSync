using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.PackageManager
{
  public interface IPluginHttpClient
  {
    //Task<string?> FetchCatalogAsync(string url, string? cachedETag = null);
    Task<IEnumerable<string>> FetchFromSourcesAsync(IEnumerable<string> sources, string? cachedETag = null);
    Task<byte[]> DownloadPluginAsync(string url, string? expectedChecksum);
  }
}
