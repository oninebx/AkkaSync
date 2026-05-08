using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using Akka.Hosting;
using AkkaSync.Abstractions;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.DataSources;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Plugins;
using AkkaSync.Core.Registries;
using AkkaSync.Infrastructure.Abstractions;
using AkkaSync.Infrastructure.Actors;
using AkkaSync.Infrastructure.Common;
using AkkaSync.Infrastructure.Messaging.Publish;
using AkkaSync.Infrastructure.StateStore;
using AkkaSync.Infrastructure.SyncPlugins.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaSync.Infrastructure.DependencyInjection;

public static class AkkaSyncExtension
{
  public static IServiceCollection AddAkkaSync(this IServiceCollection services, IConfiguration configuration, Action<AkkaSyncBuilder> syncConfigure, Config? akkaConfig = null)
  {
    services.AddSingleton<ISyncActorRegistry, SyncActorRegistry>();
    services.AddSingleton<ISyncActorResolver>(sp => new SyncActorResolver(sp.GetRequiredService<ActorSystem>()));

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

    var builder = new AkkaSyncBuilder(services, configuration);
    syncConfigure(builder);
    services.AddSingleton(builder.Options);

    services.AddSingleton<IPluginProviderRegistry<ISyncSource>, PluginProviderRegistry<ISyncSource>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncTransform>, PluginProviderRegistry<ISyncTransform>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncSink>, PluginProviderRegistry<ISyncSink>>();

    services.AddSingleton<IPluginProviderRegistryAdapter, PluginProviderRegistryAdapter<ISyncSource>>();
    services.AddSingleton<IPluginProviderRegistryAdapter, PluginProviderRegistryAdapter<ISyncTransform>>();
    services.AddSingleton<IPluginProviderRegistryAdapter, PluginProviderRegistryAdapter<ISyncSink>>();

    services.AddSingleton(sp => new ReducerRegistryBuilder()
      .AddReducer<PipelineDefinition>(PipelineReducers.ReduceDefinition)
      .AddReducer<PipelineMetrics>(PipelineReducers.ReduceMetrics)
      .AddReducer<PluginDefinition>(PluginReducers.ReduceDefinition)
      .AddReducer<PluginLocal>(PluginReducers.ReduceLocal)
      .AddReducer<PluginRemote>(PluginReducers.ReduceRemote)
      .AddReducer<PluginInstance>(PluginReducers.ReduceInstance)
      .AddReducer<ConnectorDefinition>(ConnectorReducers.ReduceDefinition)
      .Build());
    services.AddSingleton<ISnapshotStore, SnapshotStore>();

    services.AddSingleton<IContextHydrator, PluginHydrator>();

    services.AddSingleton<IEventIdGenerator, GuidEventIdGenerator>();
    services.AddSingleton<ISequenceGenerator, InMemorySequenceGenerator>();
    services.AddSingleton<IEnvelopeFactory, EnvelopeFactory>();

    services.AddAkka("AkkaSyncSystem", (builder, sp) =>
    {
      builder.WithActors((system, registry) =>
      {
        var props = DependencyResolver.For(system).Props<SyncRuntimeActor>();
        system.ActorOf(props, "sync-runtime");
      });
    });
    return services;
  }
}
