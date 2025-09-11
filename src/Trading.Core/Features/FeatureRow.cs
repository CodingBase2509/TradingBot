namespace Trading.Core.Features;

/// <summary>Feature vector for a specific symbol and timestamp.</summary>
public readonly record struct FeatureRow(
    string Symbol,
    DateTimeOffset TimestampUtc,
    FeatureSchema Schema,
    double[] Values);