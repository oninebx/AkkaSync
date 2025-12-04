using AkkaSync.Abstractions.Models;
using Xunit;

namespace AkkaSync.Abstractions.Tests.Models;

public class SyncHistoryRecordTests
{
    [Fact]
    public void SyncHistoryRecord_WithDefaults_HasCorrectInitialState()
    {
        // Arrange & Act
        var record = new SyncHistoryRecord
        {
            SourceId = "source-1"
        };

        // Assert
        Assert.Equal("source-1", record.SourceId);
        Assert.Equal("Pending", record.Status);
        Assert.Null(record.ETag);
        Assert.Null(record.Cursor);
        Assert.NotEqual(default(DateTime), record.LastSyncTimeUtc);
    }

    [Fact]
    public void SyncHistoryRecord_WithAllProperties_StoresCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var record = new SyncHistoryRecord
        {
            SourceId = "source-1",
            ETag = "etag-123",
            Cursor = "line-100",
            Status = "Running",
            LastSyncTimeUtc = now,
            ExtraDataJson = "{\"key\": \"value\"}"
        };

        // Assert
        Assert.Equal("source-1", record.SourceId);
        Assert.Equal("etag-123", record.ETag);
        Assert.Equal("line-100", record.Cursor);
        Assert.Equal("Running", record.Status);
        Assert.Equal(now, record.LastSyncTimeUtc);
        Assert.Equal("{\"key\": \"value\"}", record.ExtraDataJson);
    }

    [Fact]
    public void SyncHistoryRecord_CanUpdateStatus()
    {
        // Arrange
        var record = new SyncHistoryRecord
        {
            SourceId = "source-1",
            Status = "Pending"
        };

        // Act
        record.Status = "Completed";

        // Assert
        Assert.Equal("Completed", record.Status);
    }

    [Fact]
    public void SyncHistoryRecord_CanUpdateCursor()
    {
        // Arrange
        var record = new SyncHistoryRecord
        {
            SourceId = "source-1",
            Cursor = "line-50"
        };

        // Act
        record.Cursor = "line-100";

        // Assert
        Assert.Equal("line-100", record.Cursor);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Running")]
    [InlineData("Completed")]
    [InlineData("Failed")]
    public void SyncHistoryRecord_SupportsVariousStatuses(string status)
    {
        // Arrange & Act
        var record = new SyncHistoryRecord
        {
            SourceId = "source-1",
            Status = status
        };

        // Assert
        Assert.Equal(status, record.Status);
    }
}
