namespace QuickCLI;

internal record Parameter
{
    public bool Optional { get; init; }
    public object? DefaultValue { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init;}
    public required Type Type { get; init; }
}
