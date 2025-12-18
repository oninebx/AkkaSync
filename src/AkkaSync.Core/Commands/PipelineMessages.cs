namespace AkkaSync.Core.Messages
{
    public record StartSync();
    public record StopSync();
    public record SyncFailed(Exception Error);
}