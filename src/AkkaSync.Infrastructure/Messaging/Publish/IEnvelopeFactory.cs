using AkkaSync.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Publish
{
  public interface IEnvelopeFactory
  {
    PatchEnvelope Create(IReadOnlyList<IChangeSet> payload);
  }
}
