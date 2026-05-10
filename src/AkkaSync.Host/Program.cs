using Akka.Configuration;
using AkkaSync.Host;
using AkkaSync.Host.DependencyInjection;
using AkkaSync.Infrastructure.DependencyInjection;
using AkkaSync.Infrastructure.Realtime;
using AkkaSync.Persistence.ErrorStores;
using AkkaSync.Persistence.HistoryStore;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var logConfig = new LoggerConfiguration()
    .MinimumLevel.Information();

Config? akkaConfig = null;
if (builder.Environment.IsDevelopment())
{
  builder.Host.UseWindowsService();
  logConfig.WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30);
  Log.Logger = logConfig.CreateLogger();
  builder.Host.UseSerilog();

  var configFile = Path.Combine(AppContext.BaseDirectory, $"akka.hocon");
  akkaConfig = ConfigurationFactory.ParseString(File.ReadAllText(configFile));
}

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
      options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      options.PayloadSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddAkkaSync(builder.Configuration, sync => sync
  .AddPipelines()
  .AddPlugins()
  .UseHistoryStore<InMemoryHistoryStore>()
  .UseErrorStore<InMemoryErrorStore>(), akkaConfig);

builder.Services.AddDashboard();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.MapHub<StateHub>("/hub/state");

if (!app.Environment.IsDevelopment())
{
  var baseDir = AppContext.BaseDirectory;
  var releaseDir = Path.GetFullPath(Path.Combine(baseDir, ".."));
  var dashboardPath = Path.Combine(releaseDir, "dashboard");

    app.UseDefaultFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(dashboardPath),
        RequestPath = ""
    });

    app.MapFallbackToFile("index.html", new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(dashboardPath)
    });
}

app.Run();
