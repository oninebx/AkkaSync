using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
  public interface ISyncSink
  {
    DataSourceIdentity Identity { get; }
    Task WriteAsync(IEnumerable<TransformContext> context, CancellationToken cancellationToken);
  }
}