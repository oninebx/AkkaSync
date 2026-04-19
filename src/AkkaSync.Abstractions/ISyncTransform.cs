using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
  public interface ISyncTransform
  {
    string QualifiedId { get; }
    string Name { get; }
    Task<TransformContext> Transform(TransformContext context, CancellationToken cancellationToken);
    string Produce { get; init; }
    string[] DependsOn { get; init; }
  }
}