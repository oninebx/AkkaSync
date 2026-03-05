using Akka.Actor;
using AkkaSync.Infrastructure.Messaging.Contract.Update;
using AkkaSync.Infrastructure.Messaging.Models;
using AkkaSync.Infrastructure.SyncPlugins.PackageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AkkaSync.Infrastructure.Messaging.Contract.Update.Protocol;

namespace AkkaSync.Infrastructure.Actors
{
  public sealed class PluginUpdateActor: ReceiveActor
  {
    private readonly IPluginPackageManager _pluginPackageManager;
    public PluginUpdateActor(IPluginPackageManager packageManager) 
    {
      _pluginPackageManager = packageManager;

      Idle();
    }

    private void Idle()
    {
      Receive<CheckVersionsForUpdate>(_ =>
      {
        Become(Checking);
        Self.Tell(new DoCheck());
      });
    }

    private void Checking()
    {
      ReceiveAsync<DoCheck>(_ => HandleCheck());
    }

    private async Task HandleCheck()
    {
      try
      {
        var updates = await _pluginPackageManager.CheckoutVersions();
        if (updates.Count > 0)
        {
          Context.System.EventStream.Publish(new PluginVersionsChecked(updates));
        }
      }
      finally
      {
        Become(Idle);
      }
    }
  }
}
