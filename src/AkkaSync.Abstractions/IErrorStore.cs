using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions
{
  public interface IErrorStore
  {
    Task RecordErrorAsync(ErrorRecord error);
    Task RecordErrorsAsync(IEnumerable<ErrorRecord> errors);
    Task<IReadOnlyList<ErrorRecord>> GetErrorsAsync(ErrorQuery query);
    Task<int> CountAsync(ErrorQuery query);
    Task CleanupAsync(ErrorQuery query);
  }
}
