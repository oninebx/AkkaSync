using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins.Commands
{
  public sealed record CheckForUpdates() : IRequestQuery;
}
