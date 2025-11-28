using System;
using System.Runtime.CompilerServices;

namespace AkkaSync.Core.Pipeline;

public interface ISyncSource
{
  string Key { get; }
  IAsyncEnumerable<TransformContext> ReadAsync(CancellationToken cancellationToken);
}
