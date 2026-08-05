using NodaTime;
using TradingPlatform.Market.MarketData;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Tests.Unit.Market.MarketData;

public sealed class QuoteTests
{
    [Fact]
    public void ValidQuotePreservesItsValues()
    {
        var instrumentId = new InstrumentId("MES");
        var occurredAt = Instant.FromUtc(2026, 8, 5, 12, 0);
        var origin = new MarketDataOrigin("Databento", 42);

        var quote = new Quote(instrumentId, occurredAt, 5280.00m, 12m, 5280.25m, 8m, origin);

        Assert.Equal(instrumentId, quote.InstrumentId);
        Assert.Equal(occurredAt, quote.OccurredAt);
        Assert.Equal(5280.00m, quote.BidPrice);
        Assert.Equal(12m, quote.BidQuantity);
        Assert.Equal(5280.25m, quote.AskPrice);
        Assert.Equal(8m, quote.AskQuantity);
        Assert.Same(origin, quote.Origin);
    }

    [Theory]
    [InlineData(0, 1, 2, 1)]
    [InlineData(1, 0, 2, 1)]
    [InlineData(1, 1, 0, 1)]
    [InlineData(1, 1, 2, 0)]
    [InlineData(-1, 1, 2, 1)]
    [InlineData(1, -1, 2, 1)]
    [InlineData(1, 1, -2, 1)]
    [InlineData(1, 1, 2, -1)]
    public void NonPositivePriceOrQuantityIsRejected(
        decimal bidPrice,
        decimal bidQuantity,
        decimal askPrice,
        decimal askQuantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Quote(
            new InstrumentId("MES"),
            Instant.FromUtc(2026, 8, 5, 12, 0),
            bidPrice,
            bidQuantity,
            askPrice,
            askQuantity,
            new MarketDataOrigin("Databento")));
    }

    [Fact]
    public void CrossedQuoteRemainsAvailableForQualityEvaluation()
    {
        var quote = new Quote(
            new InstrumentId("MES"),
            Instant.FromUtc(2026, 8, 5, 12, 0),
            bidPrice: 5280.50m,
            bidQuantity: 1m,
            askPrice: 5280.25m,
            askQuantity: 1m,
            new MarketDataOrigin("Databento"));

        Assert.True(quote.BidPrice > quote.AskPrice);
    }
}
