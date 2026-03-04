using System;

namespace AkkaSync.Infrastructure.Options;

public sealed record StorageOptions
{
  public string Type { get; init; } = string.Empty;
  public string Uri { get; init; } = string.Empty;
}