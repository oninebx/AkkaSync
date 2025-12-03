using Akka.Actor;
using Akka.DependencyInjection;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Abstractions;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Examples;
using AkkaSync.Examples.TransformerPlugins;
using AkkaSync.Infrastructure;
using AkkaSync.Plugins.Sinks;
using AkkaSync.Plugins.Sinks.Factories;
using AkkaSync.Plugins.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AkkaSync.Plugins.HistoryStores;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
var config = builder.Configuration.GetSection("AkkaSync").Get<PipelineConfig>()!;

var services = new ServiceCollection();
services.AddLogging(config =>
{
  config.AddConsole();
  config.SetMinimumLevel(LogLevel.Debug);
});

services.AddSingleton<IPluginProvider<ISyncSource>, FolderWatcherSourceProvider>();
services.AddSingleton<IPluginProviderRegistry<ISyncSource>, PluginProviderRegistry<ISyncSource>>();

services.AddSingleton<IPluginProvider<ISyncTransformer>, CsvTransformerProvider>();
services.AddSingleton<IPluginProviderRegistry<ISyncTransformer>, PluginProviderRegistry<ISyncTransformer>>();

services.AddSingleton<IDatabaseSinkFactory, SqliteSinkFactory>();
services.AddSingleton<IPluginProvider<ISyncSink>, DatabaseSinkProvider>();
services.AddSingleton<IPluginProviderRegistry<ISyncSink>, PluginProviderRegistry<ISyncSink>>();

services.AddSingleton<IPluginProvider<IHistoryStore>, InMemoryHistoryStoreProvider>();
services.AddSingleton<IPluginProviderRegistry<IHistoryStore>, PluginProviderRegistry<IHistoryStore>>();

services.AddAkkaSync();

services.AddSingleton<PipelineConfig>(config);
var serviceProvider = services.BuildServiceProvider();

var boot = BootstrapSetup.Create();
var setup = DependencyResolverSetup.Create(serviceProvider);
var actorSystem = ActorSystem.Create("AkkaSyncSystem", boot.And(setup));
var resolver = DependencyResolver.For(actorSystem);



Console.WriteLine("Starting AkkaSync Demo Pipeline...");
await DemoPipeline.Run(actorSystem, resolver);
