using System;
using AkkaSync.Infrastructure.Messaging.Publish;

public class GuidEventIdGenerator : IEventIdGenerator
{
  public string NewId() => Guid.NewGuid().ToString("N");
}
