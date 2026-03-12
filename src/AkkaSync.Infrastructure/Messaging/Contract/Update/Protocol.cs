using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Contract.Update
{
  public static class Protocol
  {
    public sealed record CheckVersionsForUpdate();
    public sealed record CheckoutNewVersion(string Url, string Checksum);
    public sealed record DoCheck();
    public sealed record DoUpdate(string Url, string Checksum);
  }
}
