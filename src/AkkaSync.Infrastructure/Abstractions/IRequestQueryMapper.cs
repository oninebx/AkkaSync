using AkkaSync.Abstractions;
using System.Text.Json;

namespace AkkaSync.Infrastructure.Abstractions
{
  public interface IRequestQueryMapper
  {
    IRequestQuery? Map(QueryEnvelope envelope);
  }
  public sealed record QueryEnvelope(string Method, JsonElement Payload);
}
