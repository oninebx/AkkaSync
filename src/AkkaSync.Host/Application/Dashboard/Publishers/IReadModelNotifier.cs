using System;

namespace AkkaSync.Host.Application.Dashboard.Publishers;

public interface IReadModelNotifier<T>
{
  Task NotifyAsync(T update);
}
