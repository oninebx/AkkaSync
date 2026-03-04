using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public class InMemorySequenceGenerator : ISequenceGenerator
{
  private long _sequence = 0;
  public long Next() => Interlocked.Increment(ref _sequence);
}
