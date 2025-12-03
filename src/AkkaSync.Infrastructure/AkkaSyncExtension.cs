using System;
using AkkaSync.Core.Common;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaSync.Infrastructure;

public static class AkkaSyncExtension
{
  public static IServiceCollection AddAkkaSync(this IServiceCollection services)
  {
    services.AddSingleton<SyncGenerator>();
    return services;
  }
}
