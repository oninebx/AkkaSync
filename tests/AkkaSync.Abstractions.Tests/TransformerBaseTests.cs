using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Xunit;

namespace AkkaSync.Abstractions.Tests.Abstractions;

/// <summary>
/// Simple transformer for testing: converts a value to uppercase
/// </summary>
public class UpperCaseTransformer : TransformerBase
{
    protected override TransformContext Process(TransformContext item)
    {
        var newData = new Dictionary<string, Dictionary<string, object>>();
        foreach (var table in item.TablesData)
        {
            newData[table.Key] = new Dictionary<string, object>();
            foreach (var col in table.Value)
            {
                if (col.Value is string str)
                {
                    newData[table.Key][col.Key] = str.ToUpperInvariant();
                }
                else
                {
                    newData[table.Key][col.Key] = col.Value;
                }
            }
        }
        item.TablesData = newData;
        return item;
    }
}

/// <summary>
/// Simple transformer for testing: adds a prefix to all string values
/// </summary>
public class PrefixTransformer : TransformerBase
{
    private readonly string _prefix;

    public PrefixTransformer(string prefix = "PREFIX_")
    {
        _prefix = prefix;
    }

    protected override TransformContext Process(TransformContext item)
    {
        var newData = new Dictionary<string, Dictionary<string, object>>();
        foreach (var table in item.TablesData)
        {
            newData[table.Key] = new Dictionary<string, object>();
            foreach (var col in table.Value)
            {
                if (col.Value is string str)
                {
                    newData[table.Key][col.Key] = _prefix + str;
                }
                else
                {
                    newData[table.Key][col.Key] = col.Value;
                }
            }
        }
        item.TablesData = newData;
        return item;
    }
}

public class TransformerBaseTests
{
    [Fact]
    public void SingleTransformer_TransformsDataCorrectly()
    {
        // Arrange
        var transformer = new UpperCaseTransformer();
        var context = new TransformContext
        {
            RawData = "john,active",
            TablesData = new Dictionary<string, Dictionary<string, object>>
            {
                { "users", new Dictionary<string, object> { { "name", "john" }, { "status", "active" } } }
            },
            Cursor = "0"
        };

        // Act
        var result = transformer.Transform(context);

        // Assert
        Assert.Equal("JOHN", result.TablesData["users"]["name"]);
        Assert.Equal("ACTIVE", result.TablesData["users"]["status"]);
    }

    [Fact]
    public void ChainedTransformers_ApplyInSequence()
    {
        // Arrange
        var transformer1 = new UpperCaseTransformer();
        var transformer2 = new PrefixTransformer("PREFIX_");
        transformer1.SetNext(transformer2);

        var context = new TransformContext
        {
            RawData = "john",
            TablesData = new Dictionary<string, Dictionary<string, object>>
            {
                { "users", new Dictionary<string, object> { { "name", "john" } } }
            },
            Cursor = "0"
        };

        // Act
        var result = transformer1.Transform(context);

        // Assert
        // Should be uppercase first (JOHN), then prefixed (PREFIX_JOHN)
        Assert.Equal("PREFIX_JOHN", result.TablesData["users"]["name"]);
    }

    [Fact]
    public void Transformer_PreservesNonStringValues()
    {
        // Arrange
        var transformer = new UpperCaseTransformer();
        var context = new TransformContext
        {
            RawData = "john,123",
            TablesData = new Dictionary<string, Dictionary<string, object>>
            {
                { "users", new Dictionary<string, object> { { "name", "john" }, { "age", 123 } } }
            },
            Cursor = "0"
        };

        // Act
        var result = transformer.Transform(context);

        // Assert
        Assert.Equal("JOHN", result.TablesData["users"]["name"]);
        Assert.Equal(123, result.TablesData["users"]["age"]); // numeric value unchanged
    }

    [Fact]
    public void ThreeChainedTransformers_ApplyAllTransforms()
    {
        // Arrange
        var transformer1 = new UpperCaseTransformer();
        var transformer2 = new PrefixTransformer("PRE_");
        var transformer3 = new PrefixTransformer("_SUFFIX");
        transformer1.SetNext(transformer2);
        transformer2.SetNext(transformer3);

        var context = new TransformContext
        {
            RawData = "test",
            TablesData = new Dictionary<string, Dictionary<string, object>>
            {
                { "data", new Dictionary<string, object> { { "value", "test" } } }
            },
            Cursor = "0"
        };

        // Act
        var result = transformer1.Transform(context);

        // Assert
        // Should be: test -> TEST -> PRE_TEST -> _SUFFIXPRE_TEST
        Assert.Equal("_SUFFIXPRE_TEST", result.TablesData["data"]["value"]);
    }
}
