using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Common
{
  public interface ISyncActorRegistry
  {
    IActorRef Get<TActor>();
    void Register<TActor>(IActorRef actor);
  }
}
