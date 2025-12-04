using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface IHistoryStore
{
  Task<SyncHistoryRecord?> GetAsync(string sourceId, CancellationToken cancellationToken = default);
  Task UpsertAsync(SyncHistoryRecord record, CancellationToken cancellationToken = default);
  Task UpdateCursorAsync(string sourceId, string cursor, CancellationToken cancellationToken = default);
  Task MarkCompletedAsync(string sourceId, string etag, CancellationToken cancellationToken = default);
  Task MarkRunningAsync(string sourceId, CancellationToken cancellationToken = default);
  Task MarkFailedAsync(string sourceId, string? error = null, CancellationToken cancellationToken = default);
}


