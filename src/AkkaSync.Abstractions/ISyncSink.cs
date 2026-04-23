using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
  public interface ISyncSink
  {
    string Key { get; init; }
    string QualifiedId { get; }
    string Name { get; }
    Task<IReadOnlyList<ErrorContext>> WriteAsync(IEnumerable<TransformContext> context, CancellationToken cancellationToken);
  }
}