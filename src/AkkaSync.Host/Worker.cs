using Akka.Actor;
using AkkaSync.Infrastructure.DependencyInjection;

namespace AkkaSync.Host;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _provider;
    private ActorSystem? _actorSystem;

    public Worker(IServiceProvider provider, ILogger<Worker> logger)
    {
        _logger = logger;
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     if (_logger.IsEnabled(LogLevel.Information))
        //     {
        //         _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     }
        //     await Task.Delay(1000, stoppingToken);
        // }
      _actorSystem = _provider.RunAkkaSync();
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
    }

  public override async Task StopAsync(CancellationToken cancellationToken)
  {
    if (_actorSystem != null)
      {
          _logger.LogInformation("Stopping AkkaSync ActorSystem...");
          await _actorSystem.Terminate();
          _logger.LogInformation("AkkaSync ActorSystem terminated.");
      }
      
      await base.StopAsync(cancellationToken);
  }
}
