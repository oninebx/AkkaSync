using Akka.Streams.Implementation.Fusing;
using Akka.Util.Internal;
using AkkaSync.Infrastructure.Messaging.Models;
using AkkaSync.Infrastructure.Options;
using AkkaSync.Infrastructure.SyncPlugins.Catalog;
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
    private readonly IPluginCatalog _catalog;
    private readonly IPluginHttpClient _client;
    private readonly ILogger<GithubPackageManager> _logger;
    private Dictionary<string, PluginCatalogEntry>? _pluginsToUpdate;
    public GithubPackageManager(IPluginHttpClient client, IPluginStorage storage, IPluginCatalog catalog, IOptions<PluginOptions> options, ILogger<GithubPackageManager> logger) 
    {
      _catalogSources = options.Value.CatalogSources;
      _storage = storage;
      _catalog = catalog; ;
      _client = client;
      _logger = logger;
    }

    public async Task<string> CheckoutPlugin(string url, string checksum)
    {
      var data = await _client.DownloadPluginAsync(url, checksum);

      var uri = new Uri(url);

      var fileName = Path.GetFileName(uri.AbsolutePath);

      var filePath = await _storage.SaveAsync(fileName, new MemoryStream(data));
      var id = Path.GetFileNameWithoutExtension(fileName);
      if (_pluginsToUpdate is not null && _pluginsToUpdate.TryGetValue(id, out var entry))
      {
        await _catalog.AddAsync(new PluginCatalogEntry(id, entry.Version, false));
        _pluginsToUpdate.Remove(id);
      }
      
      return filePath;

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
      _pluginsToUpdate = _storage.Diff(registries).Select(c => new PluginCatalogEntry(c.Id, c.Version, false)).ToDictionary(c => c.Id, c => c);
      return _storage.Diff(registries);
    }
  }
}
