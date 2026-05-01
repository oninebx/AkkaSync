using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins.Models;
using AkkaSync.Infrastructure.Options;
using AkkaSync.Infrastructure.SyncPlugins.Catalog;
using AkkaSync.Infrastructure.SyncPlugins.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Versioning;
using System.Text.Json;

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

      var id = Path.GetFileNameWithoutExtension(uri.AbsolutePath);

      var filePath = await _storage.SaveAsync(id, new MemoryStream(data));
      if (_pluginsToUpdate is not null && _pluginsToUpdate.TryGetValue(id, out var entry))
      {
        await _catalog.AddAsync(new PluginCatalogEntry(id, entry.Version, entry.Checksum, false));
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

      var localPlugins = (await _catalog.GetAllAsync()).ToDictionary(p => p.Id);
      //var diff = registries.SelectMany(r => r.Plugins)
      //  .Where(p => NeedsUpdate(p, localPlugins.GetValueOrDefault(p.QualifiedName)))
      //  .ToHashSet();

      var latestPlugins = registries.SelectMany(r => r.Plugins).ToHashSet();

      _pluginsToUpdate = latestPlugins.Where(p => NeedsUpdate(p, localPlugins.GetValueOrDefault(p.QualifiedName))).Select(c => new PluginCatalogEntry(c.QualifiedName, c.Version, c.Checksum, false)).ToDictionary(c => c.Id, c => c);
      return latestPlugins;
    }

    bool NeedsUpdate(PluginPackageEntry remote, PluginCatalogEntry? local)
    {
      if (local == null)
        return true;

      var localVersion = NuGetVersion.Parse(local.Version);
      var remoteVersion = NuGetVersion.Parse(remote.Version);

      if (remoteVersion > localVersion)
        return true;

      if (remoteVersion == localVersion &&
          !string.Equals(local.Checksum, remote.Checksum, StringComparison.OrdinalIgnoreCase))
        return true;

      return false;
    }

  }
}
