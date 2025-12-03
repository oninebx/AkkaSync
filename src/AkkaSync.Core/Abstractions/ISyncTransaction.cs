using System;

namespace AkkaSync.Core.Abstractions;

public interface ISyncTransaction : IAsyncDisposable
{
  Task CommitAsync();
  Task RollbackAsync();
}
