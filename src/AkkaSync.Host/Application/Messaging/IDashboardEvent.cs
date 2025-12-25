using System;

namespace AkkaSync.Host.Application.Messaging;

public interface IDashboardEvent
{
  string TypeName { get; }
  object Payload { get; }
}
