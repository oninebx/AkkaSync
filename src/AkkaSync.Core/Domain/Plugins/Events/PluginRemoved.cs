using AkkaSync.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Domain.Plugins.Events
{
  public sealed record PluginRemoved(string QualifiedName): ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes => [typeof(PluginLocal)];
    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups => new Dictionary<Type, IReadOnlyList<string>>()
    {
      [typeof(PluginLocal)] = [QualifiedName]
    };
  }
}
