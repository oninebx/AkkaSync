using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Query.Mapper
{
  public interface IRequestQueryMapper
  {
    IRequestQuery? Map(QueryEnvelope envelope);
  }
}
