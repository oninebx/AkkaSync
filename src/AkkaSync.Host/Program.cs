using AkkaSync.Abstractions.Models;
using AkkaSync.Host;
using AkkaSync.Host.Infrastructure.Actors;
using AkkaSync.Host.Infrastructure.Extensions;
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

builder.Services.AddDashboard();

var scheduleOptions = builder.Configuration.GetSection("AkkaSync").Get<ScheduleOptions>()!;
builder.Services.AddSingleton(scheduleOptions);

var options = builder.Configuration.GetSection("AkkaSync").Get<PipelineOptions>()!;

builder.Services.AddAkkaSync((resolver, actorHooks) =>
{
  actorHooks.Insert(0, new ActorHook(resolver.Props<DashboardProxyActor>(), "dashboard-proxy"));
}).AddAkkaSyncPlugins("plugins")
.AddSingleton(options);

builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.UseCors("AllowDashboard");
app.MapHub<DashboardHub>("/hub/dashboard");
app.Run();
