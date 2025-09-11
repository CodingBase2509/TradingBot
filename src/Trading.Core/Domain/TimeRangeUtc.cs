namespace Trading.Core.Domain;

/// <summary>Inclusive UTC time range.</summary>
public readonly record struct TimeRangeUtc(
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc);