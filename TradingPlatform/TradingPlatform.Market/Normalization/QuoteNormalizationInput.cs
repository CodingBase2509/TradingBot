using NodaTime;

namespace TradingPlatform.Market.Normalization;

internal sealed record QuoteNormalizationInput(
    string? Provider,
    string? Symbol,
    Instant OccurredAt,
    decimal BidPrice,
    decimal BidQuantity,
    decimal AskPrice,
    decimal AskQuantity,
    long? SequenceNumber = null);
