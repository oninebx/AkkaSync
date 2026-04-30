namespace AkkaSync.Abstractions
{
  public interface IChangeSet
  {
    string Slice { get; }
    ChangeOperation Operation { get; }
    object Payload { get; }
  }

  public enum ChangeOperation
  {
    Upsert = 0,
    Remove = 1,
    Replace = 2
  }
}
