using System.Collections.Concurrent;

namespace AkkaSync.Abstractions.Models;

public sealed class TransformContext
{
  public IReadOnlyDictionary<string, object?> RawData { get; set; }
  public IDictionary<string, object>? MetaData { get; set; }
  public required string Cursor { get; set; }
  private IReadOnlyDictionary<string, object?> _commited;
  private readonly ConcurrentDictionary<string, object?> _pending;
  public IReadOnlyDictionary<string, object?> Artifacts => _commited;
  public ErrorContext? Error { get; set; }

  public TransformContext(IReadOnlyDictionary<string, object?> rawData)
  {
    RawData = rawData;
    _commited = new Dictionary<string, object?>();
    _pending = new ConcurrentDictionary<string, object?>();
  }

  public bool TryProduce(string artifact, object? value)
  {
    ArgumentNullException.ThrowIfNull(artifact);
    return _pending.TryAdd(artifact, value);
  }

  public void CommitLayer()
  {
    var newCommitted = new Dictionary<string, object?>(_commited);
    foreach (var kvp in _pending)
    {
      newCommitted[kvp.Key] = kvp.Value;
    }
    _commited = newCommitted;
    _pending.Clear();
  }
  
}

