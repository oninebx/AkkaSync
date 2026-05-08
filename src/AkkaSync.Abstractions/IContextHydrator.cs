using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Abstractions
{
  public interface IContextHydrator
  {
    IQueryPayload Hydrate(ISnapshotStore stateStore, IRequestQuery query);
    bool CanHydrate(IRequestQuery query);
  }
}
