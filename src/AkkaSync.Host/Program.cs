using AkkaSync.Abstractions.Models;
using AkkaSync.Host;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Infrastructure.Actors;
using AkkaSync.Host.Infrastructure.SignalR;
using AkkaSync.Host.Infrastructure.StateStore;
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
builder.Services.AddSingleton<IHostStateStore, InMomeryHostStateStore>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.UseCors("AllowDashboard");
app.MapHub<DashboardHub>("/hub/dashboard");
app.Run();
