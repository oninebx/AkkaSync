using System;
using Akka.Actor;
using AkkaSync.Abstractions;
using AkkaSync.Core.Common;
using AkkaSync.Core.PluginProviders;
using Akka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AkkaSync.Core.Actors;

namespace AkkaSync.Infrastructure.DependencyInjection;

public static class AkkaSyncExtension
{
  public static IServiceCollection AddAkkaSync(this IServiceCollection services)
  {
    services.AddSingleton<ISyncGenerator, SyncGenerator>();
    services.AddSingleton<IPluginProviderRegistry<ISyncSource>, PluginProviderRegistry<ISyncSource>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncTransformer>, PluginProviderRegistry<ISyncTransformer>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncSink>, PluginProviderRegistry<ISyncSink>>();
    services.AddSingleton<IPluginProviderRegistry<IHistoryStore>, PluginProviderRegistry<IHistoryStore>>();
    return services;
  }

  public static ActorSystem RunAkkaSync(this IServiceProvider provider)
  {
    var boot = BootstrapSetup.Create();
    var setup = DependencyResolverSetup.Create(provider);
    var actorSystem = ActorSystem.Create("AkkaSyncSystem", boot.And(setup));
    var resolver = DependencyResolver.For(actorSystem);
    actorSystem.ActorOf(resolver.Props<PipelineManagerActor>(), "PipelineManager");
    return actorSystem;
  }
}
