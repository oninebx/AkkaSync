using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using AkkaSync.Core.Abstractions;
using AkkaSync.Core.Configuration;

namespace AkkaSync.Plugins.HistoryStores;

public class InMemoryHistoryStoreProvider : IPluginProvider<IHistoryStore>
{
  public string Key => nameof(InMemoryHistoryStoreProvider);
  private readonly ConcurrentDictionary<string, Lazy<IHistoryStore>> _stores = [];

  public IEnumerable<IHistoryStore> Create(PipelineContext context, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    var key = GenerateKey(context.HistoryStoreProvider.Parameters);

    var lazyStore = _stores.GetOrAdd(key,
        _ => new Lazy<IHistoryStore>(() => new InMemorySyncHistoryStore(), 
                                      LazyThreadSafetyMode.ExecutionAndPublication)
    );

    yield return lazyStore.Value;
  }

  private static string GenerateKey(IReadOnlyDictionary<string, string> parameters)
  {
    var sorted = parameters
        .OrderBy(kv => kv.Key)
        .Select(kv => $"{kv.Key.Trim().ToLowerInvariant()}={kv.Value.Trim().ToLowerInvariant()}");

    var input = string.Join(",", sorted);
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

    return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
  }
}
