using AkkaSync.Abstractions;

namespace AkkaSync.Infrastructure.Messaging.Publish
{
  public sealed class EnvelopeFactory : IEnvelopeFactory
  {
    private readonly ISequenceGenerator _sequenceGenerator;
    private readonly IEventIdGenerator _eventIdGenerator;
    public EnvelopeFactory(ISequenceGenerator sequenceGenerator, IEventIdGenerator eventIdGenerator)
    {
      _sequenceGenerator = sequenceGenerator;
      _eventIdGenerator = eventIdGenerator;
    }
    public PatchEnvelope Create(IReadOnlyList<IChangeSet> payload)
    {
      ArgumentNullException.ThrowIfNull(payload);
      return new PatchEnvelope(_eventIdGenerator.NewId(), _sequenceGenerator.Next(), payload, DateTimeOffset.UtcNow);
    }
  }
}
