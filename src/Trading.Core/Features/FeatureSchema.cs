namespace Trading.Core.Features;

/// <summary>Defines the list and order of feature names for array-based representation.</summary>
public sealed class FeatureSchema(IReadOnlyList<string> names)
{
    public IReadOnlyList<string> Names => names;
}