using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkkaSync.Core.Pipeline
{
    public interface ISyncTransformer
    {
        TransformContext Transform(TransformContext context);
        ISyncTransformer SetNext(ISyncTransformer next);
    }
}