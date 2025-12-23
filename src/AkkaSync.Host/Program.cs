using AkkaSync.Abstractions.Models;
using AkkaSync.Host;
using AkkaSync.Host.Application.Dashboard.Publishers;
using AkkaSync.Host.Application.Dashboard.Stores;
using AkkaSync.Host.Infrastructure.Actors;
using AkkaSync.Host.Infrastructure.SignalR;
using AkkaSync.Host.Infrastructure.Stores;
using AkkaSync.Host.Web;
using AkkaSync.Infrastructure;
using AkkaSync.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddSignalR();

var config = builder.Configuration.GetSection("AkkaSync").Get<PipelineConfig>()!;
builder.Services.AddAkkaSync((resolver, actorHooks) =>
{
  actorHooks.Add(new ActorHook(resolver.Props<DashboardProxyActor>(), "dashboard-proxy"));
}).AddAkkaSyncPlugins("plugins")
.AddSingleton(config);
builder.Services.AddSingleton<IHostSnapshotPublisher, SignalRHostSnapshotPublisher>();
builder.Services.AddSingleton<IEventEnvelopePublisher, SignalREventEnvelopePublisher>();
builder.Services.AddSingleton<IHostStateStore, InMemoryHostStateStore>();
builder.Services.AddSingleton<IDashboardEventStore, InMemoryDashboardEventStore>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.UseCors("AllowDashboard");
app.MapHub<DashboardHub>("/hub/dashboard");
app.Run();
