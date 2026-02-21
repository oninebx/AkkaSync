using System;
using Akka.Actor;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Schedules;

public static class SchedulerProtocol
{
  public sealed record Initialize(IActorRef RegistryActor, ScheduleOptions Options);
  public sealed record Trigger(string Name);
}
