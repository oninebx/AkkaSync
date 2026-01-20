using System;
using Akka.Actor;

namespace AkkaSync.Core.Domain.Shared;

public static class SharedProtocol
{
  public sealed record RegisterPeer(IActorRef PeerRef);
}
