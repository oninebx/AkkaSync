using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkkaSync.Core.Pipeline
{
    public interface ISyncSink
    {
        Task WriteAsync(IEnumerable<TransformContext> context, CancellationToken cancellationToken);
    }
}