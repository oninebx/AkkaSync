using AkkaSync.Infrastructure.Messaging.Models;
using AkkaSync.Infrastructure.Options;
using AkkaSync.Infrastructure.SyncPlugins.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.PackageManager
{
  public sealed class GithubPackageManager : IPluginPackageManager
  {
    private readonly IReadOnlySet<string> _catalogSources;
    private readonly IPluginStorage _storage;
    private readonly IPluginHttpClient _client;
    public GithubPackageManager(IPluginHttpClient client, IPluginStorage storage, IOptions<PluginOptions> options) 
    {
      _catalogSources = options.Value.CatalogSources;
      _storage = storage;
      _client = client;
    }
    public async Task<IReadOnlySet<PluginVersion>> CheckVersions()
    {
      throw new NotImplementedException();
    }
  }
}
