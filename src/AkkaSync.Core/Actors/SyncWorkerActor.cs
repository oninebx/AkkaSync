using System;
using Akka.Actor;
using Akka.Event;
using Akka.Util.Internal;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Domain.Workers;
using AkkaSync.Core.Domain.Workers.Events;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Core.Actors;

public class SyncWorkerActor : ReceiveActor
{
  private readonly WorkerId _id;
  private readonly ISyncSource _source;
  private readonly IReadOnlyList<IReadOnlyList<ISyncTransform>> _transformers;
  private readonly ISyncSink _sink;
  private readonly CancellationToken _cancellationToken;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly int _batchSize;
  private readonly string? _cursor;
  private readonly Dictionary<string, int> _pluginProcessedCount;
  private readonly Dictionary<string, int> _pluginErrorCount;
  private int _metricsSinceLastFlush;
  public SyncWorkerActor(
    WorkerId id,
    ISyncSource source,
    IReadOnlyList<IReadOnlyList<ISyncTransform>> transformers,
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
    _pluginErrorCount = [];
    _pluginProcessedCount = [];
    _metricsSinceLastFlush = 0;

    ReceiveAsync<SharedProtocol.Start>(_ => StartAsync());
  }

  private async Task StartAsync()
  {
    Context.Parent.Tell(new WorkerStarted(_id));

    _cancellationToken.ThrowIfCancellationRequested();
    _logger.Info($"Worker {Self.Path.Name} started processing.");

    var batch = new List<TransformContext>();
    var errorBatch = new List<ErrorContext>();
    try
    {
      await foreach (var (context, error) in _source.ReadAsync(_cursor, _cancellationToken))
      {
        if(error is not null)
        {
          HandleError(errorBatch, [error]);
        }
        
        if (context is not null)
        {
          _pluginProcessedCount[_source.QualifiedId] = _pluginProcessedCount.GetValueOrDefault(_source.QualifiedId) + 1;
          foreach (var transformerLayer in _transformers)
          {
            var tasks = transformerLayer.Select(async t => 
            {
              var error = await t.Transform(context, _cancellationToken);
              return (pluginId: t.QualifiedId, error);
            });
            var results = await Task.WhenAll(tasks);

            foreach (var (pluginId, err) in results)
            {
              if (err is not null)
              {
                HandleError(errorBatch, [err]);
              }
              else
              {
                _pluginProcessedCount[pluginId] = _pluginProcessedCount.GetValueOrDefault(pluginId) + 1;
              }
            }
            context.CommitLayer();

          }
          batch.Add(context);
        }
        var sinkErrors = await FlushAsync(batch, _batchSize);
        if (sinkErrors.Count > 0) 
        {
          HandleError(errorBatch, sinkErrors);
        }

        _metricsSinceLastFlush++;
        
        FlushError(errorBatch, _batchSize);
        FlushMetrics(_batchSize);

      }

     
      var remainErrors = await FlushAsync(batch, 1);
      if (remainErrors.Count > 0)
      {
        HandleError(errorBatch, remainErrors);
      }
      FlushError(errorBatch, 1);
      FlushMetrics(0);


      Context.Parent.Tell(new WorkerCompleted(_id, _source.ETag));
    }
    catch (Exception ex)
    {
      Context.Parent.Tell(new WorkerFailed(_id, ex.Message));
      _logger.Error(ex, $"Worker {Self.Path.Name} encountered a fatal error: {ex.Message}");
    }
    finally
    {
      _logger.Info($"Worker {Self.Path.Name} finished processing.");
      Context.Stop(Self);
    }
  }

  private void HandleError(List<ErrorContext> batch, IReadOnlyList<ErrorContext> errors)
  {
    var errorGroup = errors.GroupBy(e => e.PluginId);
    foreach (var error in errorGroup) 
    {
      _pluginErrorCount[error.Key] = _pluginErrorCount.GetValueOrDefault(error.Key) + error.Count();
    }
    
    batch.AddRange(errors);
    _logger.Error($"Worker {Self.Path.Name} is running with error(s): {string.Join(", ", errors.Select(e => e.Message))}");
  }

  private async Task<IReadOnlyList<ErrorContext>> FlushAsync(List<TransformContext> batch, int size)
  {
    if (batch.Count >= size)
    {
      var errors = await _sink.WriteAsync(batch, _cancellationToken);
      _pluginProcessedCount[_sink.QualifiedId] = _pluginProcessedCount.GetValueOrDefault(_sink.QualifiedId) + batch.Count;
      Context.Parent.Tell(new WorkerProgressed(_id, batch.Last().Cursor));
      batch.Clear();
      return errors;
    }
    return [];
  }

  private void FlushError(List<ErrorContext> batch, int size)
  {
    if (batch.Count >= size)
    {
      Context.Parent.Tell(new WorkerErrored(_id, [.. batch.Select(e => new PluginError(e.PluginId, e.Message, e.Context))]));
      batch.Clear();
    }
  }

  void FlushMetrics(int threshhold)
  {
    if(_metricsSinceLastFlush >= threshhold)
    {
      var metrics = _pluginProcessedCount.Keys
        .Union(_pluginErrorCount.Keys)
        .Select(pluginId =>  new PluginMetrics( pluginId, _pluginProcessedCount.GetValueOrDefault(pluginId), _pluginErrorCount.GetValueOrDefault(pluginId)))
        .ToList();
      Context.System.EventStream.Publish(new WorkerMetricsReported(_id, metrics));
      //foreach (var pluginId in _pluginProcessedCount.Keys.Union(_pluginErrorCount.Keys))
      //{
      //  var metrics = new PluginMetrics(pluginId, _pluginProcessedCount.GetValueOrDefault(pluginId), _pluginErrorCount.GetValueOrDefault(pluginId));

      //  Context.System.EventStream.Publish(new WorkerMetricsReported(_id, metrics));
      //}
      _metricsSinceLastFlush = 0;
    }
    
  }

}
