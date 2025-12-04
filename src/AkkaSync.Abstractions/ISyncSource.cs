using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface ISyncSource
{
  string Id { get; }
  string Key { get; }
  string Type { get; }
  string ETag { get; }

  IAsyncEnumerable<TransformContext> ReadAsync(string? cursor, CancellationToken cancellationToken);
}
