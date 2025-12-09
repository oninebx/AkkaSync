using Akka.Actor;
using AkkaSync.Core.Messging;
using AkkaSync.Host.Messaging;
using AkkaSync.Infrastructure.DependencyInjection;

namespace AkkaSync.Host;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private ActorSystem _actorSystem;

    public Worker(ActorSystem actorSystem, ILogger<Worker> logger)
    {
      _actorSystem = actorSystem;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AkkaSync Host Worker started...");
        var dashboard = _actorSystem.ActorSelection("/user/dashboard-proxy");
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            dashboard.Tell(new DashboardEvent("heartbeat", DateTime.Now));
            await Task.Delay(1000, stoppingToken);
        }
    }

  public override async Task StopAsync(CancellationToken cancellationToken)
  {
    if (_actorSystem != null)
      {
          _actorSystem.EventStream.Publish(new HostOffline(DateTime.UtcNow, "Graceful shutdown"));
          _logger.LogInformation("Stopping AkkaSync ActorSystem...");
          await _actorSystem.Terminate();
          _logger.LogInformation("AkkaSync ActorSystem terminated.");
      }
      
      await base.StopAsync(cancellationToken);
  }
}
