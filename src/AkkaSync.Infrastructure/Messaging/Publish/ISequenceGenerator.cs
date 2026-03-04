using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public interface ISequenceGenerator
{
  long Next();
}
