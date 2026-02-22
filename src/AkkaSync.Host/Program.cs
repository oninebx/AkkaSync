using Akka.Configuration;
using Akka.Hosting;
using Akka.Logger.Serilog;
using AkkaSync.Host;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Infrastructure.Extensions;
using AkkaSync.Host.Web;
using AkkaSync.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logConfig = new LoggerConfiguration()
    .MinimumLevel.Information();

Config? akkaConfig = null;
if (!builder.Environment.IsDevelopment())
{
  builder.Host.UseWindowsService();
  logConfig.WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30);
  Log.Logger = logConfig.CreateLogger();
  builder.Host.UseSerilog();

  var configFile = Path.Combine(AppContext.BaseDirectory, $"akka.hocon");
  akkaConfig = ConfigurationFactory.ParseString(File.ReadAllText(configFile));
}

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
  .AddPlugins(), akkaConfig);

builder.Services.AddDashboard();

builder.Services.AddHostedService<Worker>();


var app = builder.Build();

app.UseCors("AllowDashboard");
app.MapHub<DashboardHub>("/hub/dashboard");
app.Run();
