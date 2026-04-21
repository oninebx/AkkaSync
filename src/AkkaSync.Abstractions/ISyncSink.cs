using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
  public interface ISyncSink
  {
    string Key { get; init; }
    string QualifiedId { get; }
    string Name { get; }
    //string Key { get; }
    Task WriteAsync(IEnumerable<TransformContext> context, CancellationToken cancellationToken);
  }
}