using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using Akka.Hosting;
using AkkaSync.Abstractions;
using AkkaSync.Core.Common;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Host.Infrastructure.Messaging;
using AkkaSync.Infrastructure.Actors;
using AkkaSync.Infrastructure.Common;
using AkkaSync.Infrastructure.Messaging.Publish;
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
    services.AddSingleton<IPluginProviderRegistry<ISyncTransformer>, PluginProviderRegistry<ISyncTransformer>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncSink>, PluginProviderRegistry<ISyncSink>>();
    services.AddSingleton<IPluginProviderRegistry<IHistoryStore>, PluginProviderRegistry<IHistoryStore>>();

    services.AddSingleton<IPluginProviderRegistryAdapter, PluginProviderRegistryAdapter<ISyncSource>>();
    services.AddSingleton<IPluginProviderRegistryAdapter, PluginProviderRegistryAdapter<ISyncTransformer>>();
    services.AddSingleton<IPluginProviderRegistryAdapter, PluginProviderRegistryAdapter<ISyncSink>>();
    services.AddSingleton<IPluginProviderRegistryAdapter, PluginProviderRegistryAdapter<IHistoryStore>>();

    services.AddSingleton<IEventIdGenerator, GuidEventIdGenerator>();
    services.AddSingleton<ISequenceGenerator, InMemorySequenceGenerator>();
    services.AddSingleton<IEventEnvelopeFactory, EventEnvelopeFactory>();

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
