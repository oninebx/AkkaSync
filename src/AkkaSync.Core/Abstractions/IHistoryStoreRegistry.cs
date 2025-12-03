using System;

namespace AkkaSync.Core.Abstractions;

public interface IHistoryStoreRegistry
{
  IHistoryStore GetStore(string type, IReadOnlyDictionary<string, string> parameters);
}
