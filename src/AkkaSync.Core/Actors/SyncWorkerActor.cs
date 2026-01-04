using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Worker;

namespace AkkaSync.Core.Actors;

public class SyncWorkerActor : ReceiveActor
{
  private readonly WorkerId _id;
  private readonly ISyncSource _source;
  private ISyncTransformer _transformer;
  private readonly ISyncSink _sink;
  private readonly CancellationToken _cancellationToken;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly int _batchSize;
  private readonly string? _cursor;
  public SyncWorkerActor(
    WorkerId id,
    ISyncSource source, 
    ISyncTransformer transformer, 
    ISyncSink sink, 
    int batchSize,
    string? cursor,
    CancellationToken cancellationToken)
  {
    _id = id;
    _source = source;
    _transformer = transformer;
    _sink = sink;
    _cancellationToken = cancellationToken;
    _batchSize = batchSize;
    _cursor = cursor;

    ReceiveAsync<WorkerProtocol.Start>(_ => StartAsync());
  }

  private async Task StartAsync()
  {
    Context.System.EventStream.Publish(new WorkerStarted(_id));

    _cancellationToken.ThrowIfCancellationRequested();
    _logger.Info($"Worker {Self.Path.Name} started processing.");

    async Task FlushAsync(List<TransformContext> batch, int size)
    {
      if(batch.Count >= size)
      {
        await _sink.WriteAsync(batch, _cancellationToken);
        Context.Parent.Tell(new WorkerProgressed(_id, batch.Last().Cursor));
        batch.Clear();
      }
    }

    var batch = new List<TransformContext>();
    try
    {
      await foreach (var context in _source.ReadAsync(_cursor, _cancellationToken))
      {
        _transformer.Transform(context);
        batch.Add(context);
        await FlushAsync(batch, _batchSize);
      }

      await FlushAsync(batch, 1);
      
      // Context.Parent.Tell(new ProcessingCompleted(Self.Path.Name, _source.Id, _source.ETag));
      Context.Parent.Tell(new WorkerCompleted(_id, _source.ETag));
    }
    catch (Exception ex)
    {
      Context.Parent.Tell(new WorkerFailed(_id, ex.Message));
      _logger.Error(ex, $"Worker {Self.Path.Name} encountered an error: {ex.Message}");
    }
    finally
    {
      _logger.Info($"Worker {Self.Path.Name} finished processing.");
      Context.Stop(Self);
    }
  }

}
