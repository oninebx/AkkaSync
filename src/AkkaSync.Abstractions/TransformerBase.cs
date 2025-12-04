using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public abstract class TransformerBase : ISyncTransformer
{
  private ISyncTransformer? _next;
  public ISyncTransformer SetNext(ISyncTransformer next)
  {
    _next = next;
    return next;
  }

  public TransformContext Transform(TransformContext item)
  {
    var processed = Process(item);
    return _next != null ? _next.Transform(processed) : processed;
  }

  protected abstract TransformContext Process(TransformContext item);
}
