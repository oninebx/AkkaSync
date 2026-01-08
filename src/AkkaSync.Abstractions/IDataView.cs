using System;

namespace AkkaSync.Abstractions;

public interface IDataView
{
  IAsyncEnumerable<IRow> Rows { get;  }
  IReadOnlyList<string> Columns { get;  }
}

public interface IRow
{
  T? Get<T>(string columnName);
}
