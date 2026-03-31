using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Common
{
  public interface ISyncActorResolver
  {
    IActorRef ActorOf<TActor>(IActorContext context, string? name = null, params object[] args) where TActor : ActorBase;
  }
}
