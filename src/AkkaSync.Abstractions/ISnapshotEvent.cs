using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Abstractions
{
  public interface ISnapshotEvent
  {
    IReadOnlyList<Type> SupportedTypes { get; }
    IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups { get;}
    DateTimeOffset OccurredAt { get => DateTimeOffset.UtcNow; }
  }
}
