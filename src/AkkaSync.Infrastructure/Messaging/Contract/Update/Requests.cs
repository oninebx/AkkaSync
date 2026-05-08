using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Infrastructure.Messaging.Contract.Update;

public class Request
{
  public sealed record UpdatePlugin(string Id) : IRequestQuery;
}
