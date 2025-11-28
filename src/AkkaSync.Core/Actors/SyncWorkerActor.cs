using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Messging;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Core.Actors;

public class SyncWorkerActor : ReceiveActor
{
  private readonly ISyncSource _source;
  private ISyncTransformer _transformer;
  private readonly ISyncSink _sink;
  private readonly CancellationToken _cancellationToken;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly int _batchSize;
  public SyncWorkerActor(ISyncSource source, ISyncTransformer transformer, ISyncSink sink, int batchSize, CancellationToken cancellationToken)
  {
    _source = source;
    _transformer = transformer;
    _sink = sink;
    _cancellationToken = cancellationToken;
    _batchSize = batchSize;
    ReceiveAsync<StartProcessing>(async _ => await RunPipeline());
  }

  override protected void PreStart() => Self.Tell(new StartProcessing());

  private async Task RunPipeline()
  {
    _cancellationToken.ThrowIfCancellationRequested();
    _logger.Info($"Worker {Self.Path.Name} started processing.");
    var batch = new List<TransformContext>();
    try
    {
      await foreach (var context in _source.ReadAsync(_cancellationToken))
      {
        _transformer.Transform(context);
        batch.Add(context);
        if (batch.Count >= _batchSize)
        {
            await _sink.WriteAsync(batch, _cancellationToken);
            batch.Clear();
        }
      }
      if (batch.Count > 0)
      {
          await _sink.WriteAsync(batch, _cancellationToken);
          batch.Clear();
      }
      Context.Parent.Tell(new ProcessingCompleted(Self.Path.Name));
    }
    catch (Exception ex)
    {
      _logger.Error(ex, $"Worker {Self.Path.Name} encountered an error: {ex.Message}");
    }
    finally
    {
      _logger.Info($"Worker {Self.Path.Name} finished processing.");
    }
  }
}
