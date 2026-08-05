using NodaTime;
using TradingPlatform.Market.MarketData;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Tests.Unit.Market.MarketData;

public sealed class TradeTests
{
    [Fact]
    public void ValidTradePreservesItsValues()
    {
        var instrumentId = new InstrumentId("MES");
        var occurredAt = Instant.FromUtc(2026, 8, 5, 12, 0);
        var origin = new MarketDataOrigin("Databento", 42);

        var trade = new Trade(instrumentId, occurredAt, 5280.25m, 3m, origin);

        Assert.Equal(instrumentId, trade.InstrumentId);
        Assert.Equal(occurredAt, trade.OccurredAt);
        Assert.Equal(5280.25m, trade.Price);
        Assert.Equal(3m, trade.Quantity);
        Assert.Same(origin, trade.Origin);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NonPositivePriceIsRejected(decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateTrade(price: price));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NonPositiveQuantityIsRejected(decimal quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateTrade(quantity: quantity));
    }

    [Fact]
    public void MissingInstrumentIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new Trade(
            null!,
            Instant.FromUtc(2026, 8, 5, 12, 0),
            5280.25m,
            1m,
            new MarketDataOrigin("Databento")));
    }

    [Fact]
    public void MissingOriginIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new Trade(
            new InstrumentId("MES"),
            Instant.FromUtc(2026, 8, 5, 12, 0),
            5280.25m,
            1m,
            null!));
    }

    private static Trade CreateTrade(decimal price = 5280.25m, decimal quantity = 1m) => new(
        new InstrumentId("MES"),
        Instant.FromUtc(2026, 8, 5, 12, 0),
        price,
        quantity,
        new MarketDataOrigin("Databento"));
}
