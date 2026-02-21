using System;

namespace AkkaSync.Abstractions;

public interface IPluginStorage
{
  string Key { get; }
  Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default);
  Task EnsureAsync(IEnumerable<string> required);
}
