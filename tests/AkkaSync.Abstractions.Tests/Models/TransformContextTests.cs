using AkkaSync.Abstractions.Models;
using Xunit;

namespace AkkaSync.Abstractions.Tests.Models;

public class TransformContextTests
{
    [Fact]
    public void TransformContext_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var context = new TransformContext
        {
            // RawData = "test,data,row",
            TablesData = new Dictionary<string, Dictionary<string, object>>
            {
                { "table1", new Dictionary<string, object> { { "col1", "value1" }, { "col2", 123 } } }
            },
            MetaData = new Dictionary<string, object> { { "key1", "value1" } },
            Cursor = "0"
        };

        // Assert
        Assert.NotNull(context);
        // Assert.Equal("test,data,row", context.RawData);
        Assert.Single(context.TablesData);
        Assert.NotNull(context.MetaData);
        Assert.Equal("0", context.Cursor);
    }

    [Fact]
    public void TransformContext_WithMultipleTables_PreservesAllData()
    {
        // Arrange & Act
        var tablesData = new Dictionary<string, Dictionary<string, object>>
        {
            { "customers", new Dictionary<string, object> { { "id", 1 }, { "name", "John" } } },
            { "orders", new Dictionary<string, object> { { "orderId", 101 }, { "amount", 99.99m } } }
        };

        var context = new TransformContext
        {
            // RawData = "1,John",
            TablesData = tablesData,
            Cursor = "1"
        };

        // Assert
        Assert.Equal(2, context.TablesData.Count);
        Assert.Contains("customers", context.TablesData.Keys);
        Assert.Contains("orders", context.TablesData.Keys);
    }
}
