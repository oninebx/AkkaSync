using System;

namespace AkkaSync.Host.Application.Messaging;

public interface ISequenceGenerator
{
  long Next();
}
