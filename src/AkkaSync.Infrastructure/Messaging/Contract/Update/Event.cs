using AkkaSync.Infrastructure.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Contract.Update
{

  public sealed record VersionsChecked(IReadOnlySet<PluginVersion> NewVersions );
}
