namespace Trading.Core.Domain;

/// <summary>Represents a single OHLCV candle for a given symbol and interval.</summary>
public readonly record struct Candle(
    long TimestampUnixMilliseconds,
    string Symbol,
    decimal OpenPrice,
    decimal HighPrice,
    decimal LowPrice,
    decimal ClosePrice,
    decimal Volume,
    bool IsClosedBar);