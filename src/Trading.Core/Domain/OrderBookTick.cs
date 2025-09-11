namespace Trading.Core.Domain;

/// <summary>Represents the best bid/ask snapshot for a symbol at a point in time.</summary>
public readonly record struct OrderBookTick(
    long TimestampUnixMilliseconds,
    string Symbol,
    decimal BestBidPrice,
    decimal BestAskPrice);