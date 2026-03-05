using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Util.Internal;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Infrastructure.SyncPlugins.PackageManager
{
  public sealed class PluginHttpClient : IPluginHttpClient
  {
    private readonly HttpClient _client;
    private readonly ILogger<PluginHttpClient> _logger;
    public PluginHttpClient(HttpClient client, ILogger<PluginHttpClient> logger)
    {
      _client = client;
      _logger = logger;
    }
    public async Task<IEnumerable<string>> FetchFromSourcesAsync(IEnumerable<string> sources, string? cachedETag = null)
    {
      var registries = new List<string>();
      foreach(var source in sources)
      {
        var registry = await _client.GetStringAsync(source);
        registries.Add(registry);
      }
      return registries;
    }
  }
}
