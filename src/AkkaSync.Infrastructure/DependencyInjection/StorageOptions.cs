using System;

namespace AkkaSync.Infrastructure.DependencyInjection;

public sealed record StorageOptions(string Type, string Uri);
