using System;

namespace AkkaSync.Host.Messaging;

public record HostOnline(DateTime Timestamp, string HostVersion, string MachineName);
public record HostOffline(DateTime Timestamp, string Reason);
