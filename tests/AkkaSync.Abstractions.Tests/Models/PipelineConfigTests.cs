using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Xunit;

namespace AkkaSync.Abstractions.Tests.Configuration;

public class PipelineConfigTests
{
    [Fact]
    public void BuildLayers_WithNoDependencies_ReturnsSingleLayer()
    {
        // Arrange
        var pipeline1 = new PipelineContext
        {
            Name = "Pipeline1",
            SourceProvider = new PluginContext { Type = "Source1" },
            TransformerProvider = new PluginContext { Type = "Transformer1" },
            SinkProvider = new PluginContext { Type = "Sink1" },
            HistoryStoreProvider = new PluginContext { Type = "Store1" },
            DependsOn = []
        };

        var pipeline2 = new PipelineContext
        {
            Name = "Pipeline2",
            SourceProvider = new PluginContext { Type = "Source2" },
            TransformerProvider = new PluginContext { Type = "Transformer2" },
            SinkProvider = new PluginContext { Type = "Sink2" },
            HistoryStoreProvider = new PluginContext { Type = "Store2" },
            DependsOn = []
        };

        var config = new PipelineConfig { Pipelines = [pipeline1, pipeline2] };

        // Act
        var layers = config.BuildLayers();

        // Assert
        Assert.Single(layers);
        Assert.Contains("Pipeline1", layers[0]);
        Assert.Contains("Pipeline2", layers[0]);
    }

    [Fact]
    public void BuildLayers_WithLinearDependencies_ReturnsMultipleLayers()
    {
        // Arrange
        var pipeline1 = new PipelineContext
        {
            Name = "Pipeline1",
            SourceProvider = new PluginContext { Type = "Source1" },
            TransformerProvider = new PluginContext { Type = "Transformer1" },
            SinkProvider = new PluginContext { Type = "Sink1" },
            HistoryStoreProvider = new PluginContext { Type = "Store1" },
            DependsOn = []
        };

        var pipeline2 = new PipelineContext
        {
            Name = "Pipeline2",
            SourceProvider = new PluginContext { Type = "Source2" },
            TransformerProvider = new PluginContext { Type = "Transformer2" },
            SinkProvider = new PluginContext { Type = "Sink2" },
            HistoryStoreProvider = new PluginContext { Type = "Store2" },
            DependsOn = ["Pipeline1"]
        };

        var pipeline3 = new PipelineContext
        {
            Name = "Pipeline3",
            SourceProvider = new PluginContext { Type = "Source3" },
            TransformerProvider = new PluginContext { Type = "Transformer3" },
            SinkProvider = new PluginContext { Type = "Sink3" },
            HistoryStoreProvider = new PluginContext { Type = "Store3" },
            DependsOn = ["Pipeline2"]
        };

        var config = new PipelineConfig { Pipelines = [pipeline1, pipeline2, pipeline3] };

        // Act
        var layers = config.BuildLayers();

        // Assert
        Assert.Equal(3, layers.Count);
        Assert.Contains("Pipeline1", layers[0]);
        Assert.Contains("Pipeline2", layers[1]);
        Assert.Contains("Pipeline3", layers[2]);
    }

    [Fact]
    public void BuildLayers_WithComplexDependencies_ReturnsCorrectLayers()
    {
        // Arrange: Pipeline1 and Pipeline2 (no deps), Pipeline3 depends on both
        var pipeline1 = new PipelineContext
        {
            Name = "Pipeline1",
            SourceProvider = new PluginContext { Type = "Source1" },
            TransformerProvider = new PluginContext { Type = "Transformer1" },
            SinkProvider = new PluginContext { Type = "Sink1" },
            HistoryStoreProvider = new PluginContext { Type = "Store1" },
            DependsOn = []
        };

        var pipeline2 = new PipelineContext
        {
            Name = "Pipeline2",
            SourceProvider = new PluginContext { Type = "Source2" },
            TransformerProvider = new PluginContext { Type = "Transformer2" },
            SinkProvider = new PluginContext { Type = "Sink2" },
            HistoryStoreProvider = new PluginContext { Type = "Store2" },
            DependsOn = []
        };

        var pipeline3 = new PipelineContext
        {
            Name = "Pipeline3",
            SourceProvider = new PluginContext { Type = "Source3" },
            TransformerProvider = new PluginContext { Type = "Transformer3" },
            SinkProvider = new PluginContext { Type = "Sink3" },
            HistoryStoreProvider = new PluginContext { Type = "Store3" },
            DependsOn = ["Pipeline1", "Pipeline2"]
        };

        var config = new PipelineConfig { Pipelines = [pipeline1, pipeline2, pipeline3] };

        // Act
        var layers = config.BuildLayers();

        // Assert
        Assert.Equal(2, layers.Count);
        Assert.Contains("Pipeline1", layers[0]);
        Assert.Contains("Pipeline2", layers[0]);
        Assert.Contains("Pipeline3", layers[1]);
    }

    [Fact]
    public void BuildLayers_WithMissingDependency_ThrowsInvalidOperationException()
    {
        // Arrange
        var pipeline1 = new PipelineContext
        {
            Name = "Pipeline1",
            SourceProvider = new PluginContext { Type = "Source1" },
            TransformerProvider = new PluginContext { Type = "Transformer1" },
            SinkProvider = new PluginContext { Type = "Sink1" },
            HistoryStoreProvider = new PluginContext { Type = "Store1" },
            DependsOn = ["NonExistentPipeline"]
        };

        var config = new PipelineConfig { Pipelines = [pipeline1] };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => config.BuildLayers());
    }

    [Fact]
    public void BuildLayers_WithCyclicDependency_ThrowsInvalidOperationException()
    {
        // Arrange: Pipeline1 depends on Pipeline2, Pipeline2 depends on Pipeline1
        var pipeline1 = new PipelineContext
        {
            Name = "Pipeline1",
            SourceProvider = new PluginContext { Type = "Source1" },
            TransformerProvider = new PluginContext { Type = "Transformer1" },
            SinkProvider = new PluginContext { Type = "Sink1" },
            HistoryStoreProvider = new PluginContext { Type = "Store1" },
            DependsOn = ["Pipeline2"]
        };

        var pipeline2 = new PipelineContext
        {
            Name = "Pipeline2",
            SourceProvider = new PluginContext { Type = "Source2" },
            TransformerProvider = new PluginContext { Type = "Transformer2" },
            SinkProvider = new PluginContext { Type = "Sink2" },
            HistoryStoreProvider = new PluginContext { Type = "Store2" },
            DependsOn = ["Pipeline1"]
        };

        var config = new PipelineConfig { Pipelines = [pipeline1, pipeline2] };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => config.BuildLayers());
    }
}
