using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Contract.Update
{
  public static class Protocol
  {
    public sealed record CheckVersions();
    public sealed record DoCheck();
  }
}
