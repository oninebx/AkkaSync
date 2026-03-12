using Akka.Actor;
using Akka.Event;
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
    private readonly ILoggingAdapter _logger = Context.GetLogger();
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

      Receive<CheckoutNewVersion>(msg =>
      {
        Become(Updating);
        Self.Tell(new DoUpdate(msg.Url, msg.Checksum));
      });
    }

    private void Checking()
    {
      ReceiveAsync<DoCheck>(_ => HandleCheck());
    }

    private void Updating()
    {
      ReceiveAsync<DoUpdate>(msg => HandleUpdate(msg.Url, msg.Checksum));
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

    private async Task HandleUpdate(string pluginUrl, string checksum)
    {
      try
      {
        var file = await _pluginPackageManager.CheckoutPlugin(pluginUrl, checksum);
        _logger.Info("Plugin in {0} is downloaded to {1}", pluginUrl, file);
      }
      catch (Exception ex)
      {
        _logger.Error("Plugin in {0) failed to download with error: {1}", pluginUrl, ex.Message);
      }
      finally
      {
        Become(Idle);
      }

    }
  }
}
