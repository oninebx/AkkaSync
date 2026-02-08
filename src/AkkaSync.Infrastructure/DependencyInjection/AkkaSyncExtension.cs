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
  public static IServiceCollection AddAkkaSync(this IServiceCollection services, Action<AkkaSyncBuilder> configure)
  {
    ISyncEnvironment env;
    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
    {
      env = SyncEnvironment.CreateDocker();
    }
    else
    {
      env = SyncEnvironment.Default();
    }
    services.AddSingleton(env);

    var builder = new AkkaSyncBuilder(services);
    configure(builder);
    services.AddSingleton(builder.Options);

    services.AddSingleton<IPluginProviderRegistry<ISyncSource>, PluginProviderRegistry<ISyncSource>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncTransformer>, PluginProviderRegistry<ISyncTransformer>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncSink>, PluginProviderRegistry<ISyncSink>>();
    services.AddSingleton<IPluginProviderRegistry<IHistoryStore>, PluginProviderRegistry<IHistoryStore>>();

    services.AddSingleton(provider =>
    {
      var bootstrap = BootstrapSetup.Create();
      var di = DependencyResolverSetup.Create(provider);
      var system = ActorSystem.Create("AkkaSyncSystem", bootstrap.And(di));
      var resolver = DependencyResolver.For(system);

      var actorHooks = builder.Options.HookActors.Select(kv => new ActorHook(resolver.Props(kv.Value), kv.Key)).ToList();

      var actorsProps = new Dictionary<string, Props>
      {
        { "pipeline-registry", resolver.Props<PipelineRegistryActor>() },
        { "pipeline-scheduler", resolver.Props<PipelineSchedulerActor>() },
        { "plugin-manager", resolver.Props<PluginManagerActor>() }
      };

      system.ActorOf(resolver.Props<SyncRuntimeActor>(actorHooks, actorsProps), "sync-runtime");

      return system;
    });


    return services;
  }
}
