using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure
{
  public static class Protocol
  {
    public sealed record LoadPlugin(string path);
  }
}
