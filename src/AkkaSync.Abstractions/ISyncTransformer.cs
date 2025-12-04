using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
    public interface ISyncTransformer
    {
        TransformContext Transform(TransformContext context);
        ISyncTransformer SetNext(ISyncTransformer next);
    }
}