using Akka.Actor;
using AkkaSync.Core.Events;
using AkkaSync.Core.Messages;

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
        // var dashboard = _actorSystem.ActorSelection("/user/dashboard-proxy");
        var random = new Random();
        while (!stoppingToken.IsCancellationRequested)
        {
        //     if (_logger.IsEnabled(LogLevel.Information))
        //     {
        //         _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     }
        //     dashboard.Tell(new DashboardEvent("heartbeat", DateTime.Now));
          // _actorSystem.EventStream.Publish(new PipelineStarted($"test-{random.Next(0, 10)}"));
          await Task.Delay(5000, stoppingToken);
        }
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
