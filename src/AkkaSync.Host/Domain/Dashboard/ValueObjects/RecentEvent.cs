using System;
using AkkaSync.Host.Application.Store;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public record RecentEvent(string Source) : IStoreValue
{

}
