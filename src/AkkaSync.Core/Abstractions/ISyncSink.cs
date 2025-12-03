using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Core.Models;

namespace AkkaSync.Core.Abstractions
{
    public interface ISyncSink
    {
        Task WriteAsync(IEnumerable<TransformContext> context, CancellationToken cancellationToken);
    }
}