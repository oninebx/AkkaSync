using System;

namespace AkkaSync.Core.Domain.Shared;

public readonly record struct RunId(Guid Value)
{
  public static RunId New() => new (Guid.NewGuid());
  public override string ToString() => Value.ToString("N");
}
