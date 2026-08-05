using NodaTime;

namespace TradingPlatform.Market.Normalization;

internal sealed record TradeNormalizationInput(
    string? Provider,
    string? Symbol,
    Instant OccurredAt,
    decimal Price,
    decimal Quantity,
    long? SequenceNumber = null);
