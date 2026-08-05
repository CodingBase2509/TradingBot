using NodaTime;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.MarketData;

public sealed record Trade
{
    internal Trade(
        InstrumentId instrumentId,
        Instant occurredAt,
        decimal price,
        decimal quantity,
        MarketDataOrigin origin)
    {
        ArgumentNullException.ThrowIfNull(instrumentId);
        ArgumentNullException.ThrowIfNull(origin);

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), price, "A trade price must be greater than zero.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "A trade quantity must be greater than zero.");
        }

        InstrumentId = instrumentId;
        OccurredAt = occurredAt;
        Price = price;
        Quantity = quantity;
        Origin = origin;
    }

    public InstrumentId InstrumentId { get; }

    public Instant OccurredAt { get; }

    public decimal Price { get; }

    public decimal Quantity { get; }

    public MarketDataOrigin Origin { get; }
}
