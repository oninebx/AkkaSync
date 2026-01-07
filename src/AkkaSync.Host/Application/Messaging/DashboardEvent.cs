using System;

namespace AkkaSync.Host.Application.Messaging;

public sealed record DashboardEvent(string TypeName, object Payload);
