using AkkaSync.Host.Application.Messaging;
using System;

namespace AkkaSync.Host.Infrastructure.Messaging;

public class GuidEventIdGenerator : IEventIdGenerator
{
  public string NewId() => Guid.NewGuid().ToString("N");
}
