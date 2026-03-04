using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.PackageManager
{
  public sealed class PluginHttpClient : IPluginHttpClient
  {

    public PluginHttpClient(HttpClient client) 
    {
    }
    public Task<string?> FetchFromSourcesAsync(IEnumerable<string> sources, string? cachedETag = null)
    {
      throw new NotImplementedException();
    }
  }
}
