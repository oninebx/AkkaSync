using System;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Application.Dashboard.Events;

public interface IDashboardEvent
{
  DateTimeOffset Timestamp { get; }
}
