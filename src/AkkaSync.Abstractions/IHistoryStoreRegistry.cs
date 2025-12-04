using System;

namespace AkkaSync.Abstractions;

public interface IHistoryStoreRegistry
{
  IHistoryStore GetStore(string type, IReadOnlyDictionary<string, string> parameters);
}
