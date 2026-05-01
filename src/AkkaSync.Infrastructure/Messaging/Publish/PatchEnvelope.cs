using AkkaSync.Abstractions;

namespace AkkaSync.Infrastructure.Messaging.Publish
{
  public sealed record PatchEnvelope(
    string Id,
    long Sequence,
    IReadOnlyList<IChangeSet> Payload,
    DateTimeOffset OccurredAt);
}
