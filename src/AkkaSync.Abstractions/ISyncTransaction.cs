using System;

namespace AkkaSync.Abstractions;

public interface ISyncTransaction : IAsyncDisposable
{
  Task CommitAsync();
  Task RollbackAsync();
}
