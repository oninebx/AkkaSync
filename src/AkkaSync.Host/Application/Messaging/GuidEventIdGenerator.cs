using System;

namespace AkkaSync.Host.Application.Messaging;

public class GuidEventIdGenerator : IEventIdGenerator
{
  public string NewId() => Guid.NewGuid().ToString("N");
}
