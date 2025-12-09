using AkkaSync.Abstractions.Models;
using AkkaSync.Host;
using AkkaSync.Host.Actors;
using AkkaSync.Host.Messaging;
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
builder.Services.AddAkkaSync((system, resolver) =>
{
  system.ActorOf(resolver.Props<DashboardProxyActor>(), "dashboard-proxy");
  system.EventStream.Publish(new HostOnline(DateTime.UtcNow, "1.0.0", Environment.MachineName));
}).AddAkkaSyncPlugins("plugins")
.AddSingleton(config);
builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.UseCors("AllowDashboard");
app.MapHub<DashboardHub>("/hub/dashboard");
app.Run();
