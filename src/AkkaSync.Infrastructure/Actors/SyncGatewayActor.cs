using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins.Commands;
using AkkaSync.Core.Domain.Plugins.Queries;
using AkkaSync.Core.Registries;
using AkkaSync.Infrastructure.Abstractions;
using AkkaSync.Infrastructure.Messaging.Publish;
using static AkkaSync.Infrastructure.Messaging.Contract.Update.Request;

namespace AkkaSync.Infrastructure.Actors;

public class SyncGatewayActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IReadOnlyDictionary<Type, IActorRef>? _routes;
  private readonly ReducerRegistry _reducerRegistry;

  private IActorRef? _pipelineManager;
  private IActorRef? _pluginManager;
  private ISyncActorRegistry _actorRegistry;
  private ISnapshotStore _snapshotStore;
  private readonly ProjectionRegistry _projectionRegistry;
  private readonly IEnvelopeFactory _envelopeFactory;
  private readonly IEnvelopePublisher _publisher;
  private readonly IEnumerable<IContextHydrator> _hydrators;
  public SyncGatewayActor(
    ISyncActorRegistry actorRegistry,
    ISnapshotStore snapshotStore,
    ReducerRegistry reducerRegistry,
    ProjectionRegistry projectionRegistry,
    IEnumerable<IContextHydrator> hydrators,
    IEnvelopeFactory envelopeFactory,
    IEnvelopePublisher publisher)
  {
    _actorRegistry = actorRegistry;
    _snapshotStore = snapshotStore;
    _hydrators = hydrators;
    _reducerRegistry = reducerRegistry;

    _reducerRegistry = reducerRegistry;
    _projectionRegistry = projectionRegistry;
    _envelopeFactory = envelopeFactory;

    _publisher = publisher;

    ReceiveAsync<ISnapshotEvent>(HandleSnapshotAsync);

    Receive<IRequestQuery>(query => HandleQuery(query));
  }
  protected override void PreStart()
  {

    _pipelineManager = _actorRegistry.Get<PipelineManagerActor>();
    _pluginManager = _actorRegistry.Get<PluginManagerActor>();
    _routes = BuildQueryRoutes(_pipelineManager, _pluginManager);

    Context.System.EventStream.Subscribe(Self, typeof(IProjectionEvent));
    Context.System.EventStream.Subscribe(Self, typeof(ISnapshotEvent));
    _logger.Info("SyncGatewayActor started.");
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(IProjectionEvent));
    Context.System.EventStream.Unsubscribe(Self, typeof(ISnapshotEvent));
  }

  private async Task HandleSnapshotAsync(ISnapshotEvent @event)
  {
    var nexts = new List<ISnapshot>();
    var idGroups = @event.IdGroups;
    var changesToSend = new List<IChangeSet>();
    //foreach(var type in @event.ResetTypes)
    //{
    //  var currents = _snapshotStore.GetCurrentByType(type);
    //  var oldSnapshots = currents.Values.ToList();
    //  _snapshotStore.ResetByType(type);
    //  if (_projectionRegistry.TryProjection(type, oldSnapshots, nexts, out var changes))
    //  {
    //    changesToSend.AddRange(changes);
    //  }
    //}
    //if (changesToSend.Count > 0)
    //{
    //  var envelope = _envelopeFactory.Create(changesToSend);
    //  await _publisher.PublishAsync(envelope);
    //}

    //changesToSend.Clear();

    foreach (var type in @event.SupportedTypes)
    {
      var currents = _snapshotStore.GetCurrentByType(type);
      var oldSnapshots = currents.Values.ToList();
      var ids = idGroups[type];
      foreach (var id in ids) 
      {
        currents.TryGetValue(id, out var current);

        if (_reducerRegistry.TryReduce(type, id, current, @event, out var next))
        {
          nexts.Add(next);
        }
      }
      _snapshotStore.Update(nexts);

      if ( _projectionRegistry.TryProjection(type, oldSnapshots, nexts, out var changes))
      {
        changesToSend.AddRange(changes);
      }
      nexts.Clear();
    }
    if (changesToSend.Count > 0)
    {
      var envelope = _envelopeFactory.Create(changesToSend);
      await _publisher.PublishAsync(envelope);
    }
    
  }

  private void HandleQuery(IRequestQuery msg)
  {
    foreach(var hydrator in _hydrators)
    {
      if (hydrator.CanHydrate(msg))
      {
        var payload = hydrator.Hydrate(_snapshotStore, msg);
        if(_routes!.TryGetValue(payload.GetType(), out var target))
        {
          target.Forward(payload);
        }
        else
        {
          _logger.Warning("No route matches {0}", payload.GetType().Name);
        }
      }
    }
  }

  private IReadOnlyDictionary<Type, IActorRef> BuildQueryRoutes(IActorRef pipeline, IActorRef plugin)
  {
    var routes = new Dictionary<Type, IActorRef>()
    {
      { typeof(CheckVersionsQuery), plugin },
      { typeof(DownloadQuery), plugin },
    };
    
    return routes;
  }
}
