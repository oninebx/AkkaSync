using AkkaSync.Abstractions.Models;
using AkkaSync.Host;
using AkkaSync.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration.GetSection("AkkaSync").Get<PipelineConfig>()!;
builder.Services.AddHostedService<Worker>();

builder.Services.AddAkkaSync().AddAkkaSyncPlugins("plugins").AddSingleton(config);

var host = builder.Build();
host.Run();
