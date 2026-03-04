using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public interface IEventIdGenerator
{
  string NewId();
}
