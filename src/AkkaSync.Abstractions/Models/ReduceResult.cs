namespace AkkaSync.Abstractions.Models
{
  public sealed record ReduceResult<TState>(TState NewState, IChangeSet? Change) where TState: IStateSnashot;
}
