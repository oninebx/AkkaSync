using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public record RecentEvent(string Source) : IStoreValue
{

}
