using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
    public interface ISyncTransformer
    {
        Task<TransformContext> Transform(TransformContext context, CancellationToken cancellationToken);
        // ISyncTransformer SetNext(ISyncTransformer next);
        string Produce { get; init; }
        string[] DependsOn { get; init; }
    }
}