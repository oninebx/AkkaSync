using AkkaSync.Core.Domain.DataSources;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Plugins;
using AkkaSync.Core.Registries;
using AkkaSync.Host.Application.DataSources;
using AkkaSync.Host.Application.Pipelines;
using AkkaSync.Host.Application.Plugins;
using AkkaSync.Infrastructure.Abstractions;
using AkkaSync.Infrastructure.Realtime;

namespace AkkaSync.Host.DependencyInjection;

public static class DashboardServiceExtension
{
  public static IServiceCollection AddDashboard(this IServiceCollection services)
  {

    services.AddSingleton(sp => new ProjectionRegistryBuilder()
      .Add<PipelineDefinition>(PipelineProjections.ProjectionDefinition)
      .Add<PipelineMetrics>(PipelineProjections.ProjectionMetrics)
      .Add<PluginDefinition>(PluginProjections.ProjectionDefinition)
      .Add<PluginLocal>(PluginProjections.ProjectionLocal)
      .Add<PluginRemote>(PluginProjections.ProjectionRemote)
      .Add<PluginInstance>(PluginProjections.ProjectionInstance)
      .Add<ConnectorDefinition>(ConnectorProjections.ProjectionDefinition)
      .Build());

    services.AddSingleton<IHubCallerRegistry, SignalRCallerRegistry>();
    services.AddSingleton<IEnvelopePublisher, SignalRPublisher>();

    services.AddSingleton<IRequestQueryMapper, RequestQueryMapper>();
    return services;
  }
}
