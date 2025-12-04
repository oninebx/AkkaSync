using System;
using System.Collections.Concurrent;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Plugins.HistoryStore.InMemory;

public class InMemoryHistoryStore : IHistoryStore
{
  private readonly ConcurrentDictionary<string, SyncHistoryRecord> _cache = [];
  public Task<SyncHistoryRecord?> GetAsync(string sourceId, CancellationToken cancellationToken = default)
  {
    _cache.TryGetValue(sourceId, out var record);
    return Task.FromResult(record);
  }

  public Task MarkCompletedAsync(string sourceId, string etag, CancellationToken cancellationToken = default)
  {
    _cache.AddOrUpdate(sourceId, new SyncHistoryRecord 
    {
       SourceId = sourceId,
       ETag = etag,
       Status = "Completed",
       LastSyncTimeUtc = DateTime.UtcNow
    }, (_, existing) => { existing.ETag = etag; existing.Status = "Completed"; existing.LastSyncTimeUtc = DateTime.UtcNow; return existing; });
    return Task.CompletedTask;
  }

  public Task MarkFailedAsync(string sourceId, string? error = null, CancellationToken cancellationToken = default)
  {
    _cache.AddOrUpdate(sourceId, new SyncHistoryRecord 
    { 
      SourceId = sourceId, 
      Status = "Failed", 
      ExtraDataJson = error, 
      LastSyncTimeUtc = DateTime.UtcNow 
    },
    (_, existing) => { existing.Status = "Failed"; existing.ExtraDataJson = error; existing.LastSyncTimeUtc = DateTime.UtcNow; return existing; });
    return Task.CompletedTask;
  }

  public Task MarkRunningAsync(string sourceId, CancellationToken cancellationToken = default)
  {
    _cache.AddOrUpdate(sourceId, new SyncHistoryRecord 
    { 
      SourceId = sourceId, 
      Status = "Running", 
      LastSyncTimeUtc = DateTime.UtcNow 
    }, (_, existing) => { existing.Status = "Running"; existing.LastSyncTimeUtc = DateTime.UtcNow; return existing; });
    return Task.CompletedTask;
  }

  public Task UpdateCursorAsync(string sourceId, string cursor, CancellationToken cancellationToken = default)
  {
    _cache.AddOrUpdate(sourceId, new SyncHistoryRecord 
    { 
      SourceId = sourceId, 
      Cursor = cursor, 
      LastSyncTimeUtc = DateTime.UtcNow, 
      Status = "Running" 
    },
    (_, existing) => { existing.Cursor = cursor; existing.LastSyncTimeUtc = DateTime.UtcNow; existing.Status = "Running"; return existing; });
    return Task.CompletedTask;
  }

  public Task UpsertAsync(SyncHistoryRecord record, CancellationToken cancellationToken = default)
  {
    _cache.AddOrUpdate(record.SourceId, record, (_, __) => record);
    return Task.CompletedTask;
  }
}
