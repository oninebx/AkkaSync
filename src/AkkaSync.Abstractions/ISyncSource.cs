using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface ISyncSource: IPlugin
{
  string Type { get; }
  string ETag { get; }

  IAsyncEnumerable<(TransformContext? context, ErrorContext? error)> ReadAsync(string? cursor, CancellationToken cancellationToken);
}
