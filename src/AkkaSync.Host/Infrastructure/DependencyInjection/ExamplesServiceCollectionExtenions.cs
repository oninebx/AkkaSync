using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Host.Infrastructure.DependencyInjection;

public static class ExamplesServiceCollectionExtenions
{
  public static void AddExamples(this IServiceCollection services, IConfiguration configuration)
  {
    var enabledPipelines = configuration.GetSection("Pipelines").Get<string[]>() ?? [];

    var pipelinesConfig = PipelineLoader.LoadFromFiles("pipelines", enabledPipelines);

    var mergedConfig = new ConfigurationBuilder()
        .AddConfiguration(pipelinesConfig)
        .AddConfiguration(configuration)
        .Build();

    var scheduleOptions = mergedConfig.GetSection("AkkaSync").Get<ScheduleOptions>()!;
    services.AddSingleton(scheduleOptions);
    var pipelineOptions = mergedConfig.GetSection("AkkaSync").Get<PipelineOptions>()!;
    services.AddSingleton(pipelineOptions);
  }
}
