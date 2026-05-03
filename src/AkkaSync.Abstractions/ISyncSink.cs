using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
  public interface ISyncSink: IPlugin
  {
    Task<IReadOnlyList<ErrorContext>> WriteAsync(IEnumerable<TransformContext> context, CancellationToken cancellationToken);
  }
}