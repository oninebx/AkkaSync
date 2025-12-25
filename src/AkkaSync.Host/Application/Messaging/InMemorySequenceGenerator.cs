using System;

namespace AkkaSync.Host.Application.Messaging;

public class InMemorySequenceGenerator : ISequenceGenerator
{
  private long _sequence = 0;
  public long Next() => Interlocked.Increment(ref _sequence);
}
