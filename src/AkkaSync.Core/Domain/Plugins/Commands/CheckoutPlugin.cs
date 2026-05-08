using AkkaSync.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Domain.Plugins.Commands
{
  public sealed record CheckoutPlugin(string Id): IRequestQuery
  {
  }
}
