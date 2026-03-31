using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using System.Collections.Concurrent;

namespace AkkaSync.Persistence.ErrorStores
{
  public sealed class InMemoryErrorStore : IErrorStore
  {
    private readonly ConcurrentBag<ErrorRecord> _errors = new();
    public Task CleanupAsync(ErrorQuery query)
    {
      return Task.CompletedTask;
    }

    public Task<int> CountAsync(ErrorQuery query)
    {
      var count = _errors.Count(e =>
            (query.PipelineId == null || e.PId == query.PipelineId) &&
            (query.PluginId == null || e.PluginId == query.PluginId)
        );

      return Task.FromResult(count);
    }

    public Task<IReadOnlyList<ErrorRecord>> GetErrorsAsync(ErrorQuery query)
    {
      var result = _errors
            .Where(e =>
                (query.PipelineId == null || e.PId == query.PipelineId) &&
                (query.PluginId == null || e.PluginId == query.PluginId) &&
                (query.From == null || e.TimestampUtc >= query.From) &&
                (query.To == null || e.TimestampUtc <= query.To)
            )
            .ToList();

      return Task.FromResult<IReadOnlyList<ErrorRecord>>(result);
    }

    public Task RecordErrorAsync(ErrorRecord error)
    {
      _errors.Add(error);
      return Task.CompletedTask;
    }

    public Task RecordErrorsAsync(IEnumerable<ErrorRecord> errors)
    {
      if (errors == null) return Task.CompletedTask;

      foreach (var error in errors)
      {
        _errors.Add(error);
      }

      return Task.CompletedTask;
    }
  }
}
