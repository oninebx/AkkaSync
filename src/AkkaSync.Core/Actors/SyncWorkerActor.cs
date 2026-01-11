using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Workers;
using AkkaSync.Core.Domain.Workers.Events;

namespace AkkaSync.Core.Actors;

public class SyncWorkerActor : ReceiveActor
{
  private readonly WorkerId _id;
  private readonly ISyncSource _source;
  private readonly IReadOnlyList<IReadOnlyList<ISyncTransformer>> _transformers;
  private readonly ISyncSink _sink;
  private readonly CancellationToken _cancellationToken;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly int _batchSize;
  private readonly string? _cursor;
  public SyncWorkerActor(
    WorkerId id,
    ISyncSource source,
    IReadOnlyList<IReadOnlyList<ISyncTransformer>> transformers,
    ISyncSink sink,
    int batchSize,
    string? cursor,
    CancellationToken cancellationToken)
  {
    _id = id;
    _source = source;
    _transformers = transformers;
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
      if (batch.Count >= size)
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
        foreach (var transformerLayer in _transformers)
        {
          await Task.WhenAll(
            transformerLayer.Select(t => t.Transform(context, _cancellationToken))
          );
          context.CommitLayer();
        }
        batch.Add(context);
        await FlushAsync(batch, _batchSize);
      }

      await FlushAsync(batch, 1);

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
