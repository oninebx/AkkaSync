using System;

namespace AkkaSync.Host.Application.Messaging;

public interface IEventIdGenerator
{
  string NewId();
}
