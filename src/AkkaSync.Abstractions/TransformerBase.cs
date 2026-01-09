using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public abstract class TransformerBase : ISyncTransformer
{
  public IReadOnlySet<string> Produces { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
  public IReadOnlySet<string> DependsOn { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
  public string Produce { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
  string[] ISyncTransformer.DependsOn { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

  public TransformContext Transform(TransformContext context, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  // private ISyncTransformer? _next;
  // public ISyncTransformer SetNext(ISyncTransformer next)
  // {
  //   _next = next;
  //   return next;
  // }

  // public TransformContext Transform(TransformContext item)
  // {
  //   var processed = Process(item);
  //   return _next != null ? _next.Transform(processed) : processed;
  // }

  protected abstract TransformContext Process(TransformContext item);

  Task<TransformContext> ISyncTransformer.Transform(TransformContext context, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
