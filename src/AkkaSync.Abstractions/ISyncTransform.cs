using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
  public interface ISyncTransform: IPlugin
  {
    Task<ErrorContext?> Transform(TransformContext context, CancellationToken cancellationToken);
    string Produce { get; init; }
    string[] DependsOn { get; init; }
  }
}