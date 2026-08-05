using NodaTime;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.MarketData;

public sealed record Quote
{
    internal Quote(
        InstrumentId instrumentId,
        Instant occurredAt,
        decimal bidPrice,
        decimal bidQuantity,
        decimal askPrice,
        decimal askQuantity,
        MarketDataOrigin origin)
    {
        ArgumentNullException.ThrowIfNull(instrumentId);
        ArgumentNullException.ThrowIfNull(origin);

        EnsurePositive(bidPrice, nameof(bidPrice), "A bid price must be greater than zero.");
        EnsurePositive(bidQuantity, nameof(bidQuantity), "A bid quantity must be greater than zero.");
        EnsurePositive(askPrice, nameof(askPrice), "An ask price must be greater than zero.");
        EnsurePositive(askQuantity, nameof(askQuantity), "An ask quantity must be greater than zero.");

        InstrumentId = instrumentId;
        OccurredAt = occurredAt;
        BidPrice = bidPrice;
        BidQuantity = bidQuantity;
        AskPrice = askPrice;
        AskQuantity = askQuantity;
        Origin = origin;
    }

    public InstrumentId InstrumentId { get; }

    public Instant OccurredAt { get; }

    public decimal BidPrice { get; }

    public decimal BidQuantity { get; }

    public decimal AskPrice { get; }

    public decimal AskQuantity { get; }

    public MarketDataOrigin Origin { get; }

    private static void EnsurePositive(decimal value, string parameterName, string message)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, message);
        }
    }
}
