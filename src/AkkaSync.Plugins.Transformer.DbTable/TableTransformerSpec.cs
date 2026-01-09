using System;

namespace AkkaSync.Plugins.Transformer.DbTable;

public sealed record TableTransformerSpec
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string[] Fields { get; init; } = [];
    public string[] Columns { get; init; } = [];
    public string Produce { get; init; } = string.Empty;
    public string[] DependsOn { get; init; } = [];
}