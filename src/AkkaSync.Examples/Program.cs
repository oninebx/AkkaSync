using Akka.Actor;
using Akka.DependencyInjection;
using AkkaSync.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AkkaSync.Abstractions.Models;
using AkkaSync.Plugins.HistoryStore.InMemory;
using AkkaSync.Plugins.Sink.Sqlite;
using AkkaSync.Infrastructure.DependencyInjection;
using AkkaSync.Plugins.Source.File;
using AkkaSync.Plugins.Transformer.examples;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
var pipelineOptions = builder.Configuration.GetSection("AkkaSync").Get<PipelineOptions>()!;

var services = new ServiceCollection();
services.AddLogging(config =>
{
  config.AddConsole();
  config.SetMinimumLevel(LogLevel.Debug);
});

services.AddSingleton<IPluginProvider<ISyncTransformer>, CsvTransformerProvider>();
services.AddAkkaSync()
        .AddSqliteSink()
        .AddFileSource()
        .AddInMemoryHistoryStore();

services.AddSingleton(pipelineOptions);
var serviceProvider = services.BuildServiceProvider();

// var actorSystem = serviceProvider.RunAkkaSync();

Console.WriteLine("Starting AkkaSync Demo Pipeline...");

Console.WriteLine("Press Enter to exit...");
Console.ReadLine();

// await actorSystem.Terminate();
Console.WriteLine("AkkaSync Demo Ends");
