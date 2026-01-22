using System.Diagnostics;
using AkkaSync.Abstractions.Models;
using AkkaSync.Host;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Infrastructure;
using AkkaSync.Host.Infrastructure.DependencyInjection;
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

DataPath.Initialize();
Debug.WriteLine($"AkkaSync DataRoot: {DataPath.DataRoot}");

builder.Services.AddSignalR();
builder.Services.AddAkkaSync((resolver, actorHooks) =>
{
  actorHooks.Insert(0, new ActorHook(resolver.Props<DashboardProxyActor>(), "dashboard-proxy"));
}).AddAkkaSyncPlugins("plugins");
builder.Services.AddDashboard();
builder.Services.AddExamples(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseCors("AllowDashboard");
app.MapHub<DashboardHub>("/hub/dashboard");
app.Run();
