using System;

namespace AkkaSync.Infrastructure.SyncPlugins.Storage;

public interface IPluginStorage
{
  string Key { get; }
  Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default);
  Task EnsureAsync(IEnumerable<string> required);
}
