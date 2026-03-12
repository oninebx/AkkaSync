using Akka.Util.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

    public async Task<byte[]> DownloadPluginAsync(string url, string expectedChecksum)
    {
      _logger.LogInformation("Downloading plugin from {Url}", url);
      var response = await _client.GetAsync(url);
      response.EnsureSuccessStatusCode();

      var bytes = await response.Content.ReadAsByteArrayAsync();

      using var sha256 = SHA256.Create();
      var hashBytes = sha256.ComputeHash(bytes);
      var actualChecksum = Convert.ToHexString(hashBytes);

      if (!string.Equals(actualChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase))
      {
        _logger.LogError(
            "Plugin checksum mismatch. Expected: {Expected}, Actual: {Actual}",
            expectedChecksum,
            actualChecksum
        );

        throw new InvalidOperationException("Plugin checksum validation failed.");
      }

      _logger.LogInformation("Plugin downloaded and checksum verified.");

      return bytes;
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
