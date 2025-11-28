using System;

namespace AkkaSync.Core.Pipeline;

public interface ISyncTransaction : IAsyncDisposable
{
  Task CommitAsync();
  Task RollbackAsync();
}
