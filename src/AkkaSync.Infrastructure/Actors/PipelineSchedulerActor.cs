using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Schedules;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Runtime;
using AkkaSync.Infrastructure.Abstractions;
using NCrontab;

namespace AkkaSync.Infrastructure.Actors;

public class PipelineSchedulerActor : ReceiveActor
{
  private IReadOnlyDictionary<string, IReadOnlySet<string>>? _schedules;
  private readonly Dictionary<string, ICancelable> _jobs = [];
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _pipelineRegistry;
  private ISyncActorRegistry _actorRegistry;
  public PipelineSchedulerActor(ISyncActorRegistry actorRegistry)
  {
    _actorRegistry = actorRegistry;

    Receive<SchedulerProtocol.Initialize>(msg =>
    {
      _schedules = msg.Schedules;
      _logger.Info("{0} actor initialized at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      Context.Parent.Tell(new SchedulerInitialized());
    });

    Receive<SharedProtocol.Start>(_ => HandleStart());

    Receive<SchedulerProtocol.Trigger>(msg =>
    {
      _logger.Info($"Pipeline triggered: { msg.Key }");
      _pipelineRegistry.Tell(new RegistryProtocol.CreatePipeline(msg.Key));
      var spec = _schedules!.FirstOrDefault(s => s.Key.Contains(msg.Key)).Value;
      Context.System.EventStream.Publish(new PipelineTriggered(msg.Key));
    });

    Receive<PipelineCompleted>(msg => HandlePipeline(msg.PipelineId));
    Receive<PipelineSkipped>(msg => HandlePipeline(msg.PipelineId));
  }

  private void HandlePipeline(PipelineId id)
  {
    var crons = _schedules![id.Key];
    var nextUtc = ScheduleNextRun(id.Key, crons);
    if (nextUtc is DateTime scheduledTime)
    {
      Context.System.EventStream.Publish(new PipelineScheduled(id.Key, scheduledTime));
    }
    
  }

  private DateTime? ScheduleNextRun(string key, IReadOnlySet<string> crons)
  {
    
    var now = DateTime.UtcNow;
    var nextUtc = crons
        .Select(cron => (DateTime?)CrontabSchedule.Parse(cron).GetNextOccurrence(now))
        .DefaultIfEmpty()
        .Min();
    if (nextUtc is DateTime scheduledTime)
    {
      var delay = scheduledTime - now;

      var cancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(
        delay,
        Self,
        new SchedulerProtocol.Trigger(key),
        Self
      );

      if (_jobs.TryGetValue(key, out var job))
      {
        job.Cancel();
      }
      _jobs[key] = cancelable;
      _logger.Info("{0} will run at {1}. Please wait for {2}.", key, nextUtc, delay);
    }
   

    return nextUtc;
  }

  private void HandleStart()
  {
    if (_schedules == null)
    {
      _logger.Warning("PipelineSchedulerActor received Start message before initialization. Ignoring.");
      return;
    }
    foreach (var schedule in _schedules)
    {
      var nextUtc = ScheduleNextRun(schedule.Key, schedule.Value);
      if(nextUtc is DateTime scheduledTime)
      {
        Context.System.EventStream.Publish(new PipelineScheduled(schedule.Key, scheduledTime));
      }
      
    }
  }

  protected override void PreStart()
  {
    _pipelineRegistry = _actorRegistry.Get<PipelineRegistryActor>();
  }
  protected override void PostStop()
  {
    foreach(var job in _jobs.Values)
    {
      job.Cancel();
    }
    _logger.Info("{0} actor stopped at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
  }
}
