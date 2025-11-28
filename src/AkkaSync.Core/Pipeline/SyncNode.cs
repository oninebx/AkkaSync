using System;

namespace AkkaSync.Core.Pipeline;

public class SyncNode(string name)
{
  public string Name { get; init; } = name;
  public List<string> DependsOn { get; init; } = [];
}
