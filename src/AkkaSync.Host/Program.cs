using AkkaSync.Host;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Infrastructure.Extensions;
using AkkaSync.Host.Web;
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

builder.Services.AddAkkaSync(builder.Configuration, sync => sync
  .AddActorHook<DashboardProxyActor>("dashboard-proxy")
  .AddPipelines()
  .AddPlugins());

builder.Services.AddDashboard();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseCors("AllowDashboard");
app.MapHub<DashboardHub>("/hub/dashboard");
app.Run();
