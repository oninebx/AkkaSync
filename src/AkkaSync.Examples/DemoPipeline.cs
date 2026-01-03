using System;
using Akka.Actor;
using Akka.DependencyInjection;
using AkkaSync.Core.Actors;

namespace AkkaSync.Examples;

public static class DemoPipeline
{
  public static async Task Run(ActorSystem actorSystem, DependencyResolver resolver)
  {
    
    var manager = actorSystem.ActorOf(resolver.Props<PipelineManagerActor>(), "PipelineManager");
    

    await Task.Delay(2000);
    
    // manager.Tell(new StopPipeline("CustomerCsvToSqlite"));
    // manager.Tell(new StopPipeline("PaymentCsvToSqlite"));

    await actorSystem.Terminate();
  }
}
