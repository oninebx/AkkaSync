using AkkaSync.Infrastructure.Messaging.Publish;
namespace AkkaSync.Infrastructure.Abstractions
{
  public interface IEnvelopePublisher
  {
    Task PublishAsync(PatchEnvelope envelope);
  }
}
