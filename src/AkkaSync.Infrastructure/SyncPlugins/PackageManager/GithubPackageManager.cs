using Akka.Streams.Implementation.Fusing;
using Akka.Util.Internal;
using AkkaSync.Infrastructure.Messaging.Models;
using AkkaSync.Infrastructure.Options;
using AkkaSync.Infrastructure.SyncPlugins.Models;
using AkkaSync.Infrastructure.SyncPlugins.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.PackageManager
{
  public sealed class GithubPackageManager : IPluginPackageManager
  {
    private readonly IReadOnlySet<string> _catalogSources;
    private readonly IPluginStorage _storage;
    private readonly IPluginHttpClient _client;
    private readonly ILogger<GithubPackageManager> _logger;
    public GithubPackageManager(IPluginHttpClient client, IPluginStorage storage, IOptions<PluginOptions> options, ILogger<GithubPackageManager> logger) 
    {
      _catalogSources = options.Value.CatalogSources;
      _storage = storage;
      _client = client;
      _logger = logger;
    }
    public async Task<IReadOnlySet<PluginPackageEntry>> CheckoutVersions()
    {
      var registries = new HashSet<PluginPackageRegistry>();
      var registryStrings = await _client.FetchFromSourcesAsync(_catalogSources);
      var options = new JsonSerializerOptions
      {
          PropertyNameCaseInsensitive = true
      };
      foreach(var registry in registryStrings)
      {
        var pluginsInRegistry = JsonSerializer.Deserialize<PluginPackageRegistry>(registry, options);
        if(pluginsInRegistry is not null)
        {
          registries.Add(pluginsInRegistry);
        }
      }
      return _storage.Diff(registries);;
    }
  }
}
