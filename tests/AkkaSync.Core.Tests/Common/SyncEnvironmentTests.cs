using AkkaSync.Core.Common;
using Xunit;

namespace AkkaSync.Core.Tests.Common;

public class SyncEnvironmentTests
{
    [Fact]
    public void ComputeSha256_WithSingleValue_GeneratesValidHash()
    {
        // Arrange
        var environment = SyncEnvironment.Default();
        var input = "test-string";

        // Act
        var hash = environment.ComputeSha256(input);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.Equal(64, hash.Length); // SHA256 produces 64 hex characters
        Assert.Matches(@"^[a-f0-9]+$", hash); // Only lowercase hex characters
    }

    [Fact]
    public void ComputeSha256_WithMultipleValues_CombinesAndHashesThem()
    {
        // Arrange
        var environment = SyncEnvironment.Default();

        // Act
        var hash1 = environment.ComputeSha256("value1", "value2");
        var hash2 = environment.ComputeSha256("value1,value2"); // These produce same hash since they're combined with comma

        // Assert - They should be the same because implementation joins with comma
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeSha256_WithIdenticalInputs_ProducesSameHash()
    {
        // Arrange
        var environment = SyncEnvironment.Default();
        var input = "test-value";

        // Act
        var hash1 = environment.ComputeSha256(input);
        var hash2 = environment.ComputeSha256(input);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeSha256_WithDifferentInputs_ProducesDifferentHash()
    {
        // Arrange
        var environment = SyncEnvironment.Default();

        // Act
        var hash1 = environment.ComputeSha256("value1");
        var hash2 = environment.ComputeSha256("value2");

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ComputeSha256_WithEmptyStringsInParams_IgnoresThem()
    {
        // Arrange
        var environment = SyncEnvironment.Default();

        // Act
        var hash1 = environment.ComputeSha256("value1", "", "value2");
        var hash2 = environment.ComputeSha256("value1", "value2");

        // Assert - Should be the same because empty strings are filtered out
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeSha256_WithWhitespaceStrings_TrimsBeforeHashing()
    {
        // Arrange
        var environment = SyncEnvironment.Default();

        // Act
        var hash1 = environment.ComputeSha256("  value1  ", "  value2  ");
        var hash2 = environment.ComputeSha256("value1", "value2");

        // Assert - Should be the same because whitespace is trimmed
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeSha256_WithNoValidValues_ThrowsArgumentException()
    {
        // Arrange
        var environment = SyncEnvironment.Default();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => environment.ComputeSha256());
    }

    [Fact]
    public void ComputeSha256_WithEmptyStringInParams_IgnoresThem()
    {
        // Arrange
        var environment = SyncEnvironment.Default();

        // Act - empty strings are filtered by LINQ Where
        var hash1 = environment.ComputeSha256("value1", "");
        var hash2 = environment.ComputeSha256("value1");

        // Assert - They should be the same
        Assert.Equal(hash1, hash2);
    }
}
