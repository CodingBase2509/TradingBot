using NodaTime;
using TradingPlatform.Market.DataSources;

namespace TradingPlatform.Tests.Unit.Market.DataSources;

public sealed class MarketDataSourceRequestTests
{
    [Fact]
    public void HistoricalRequestPreservesAnImmutableSnapshot()
    {
        var symbols = new List<string> { "MES.FUT" };
        var from = Instant.FromUtc(2026, 8, 5, 12, 0);
        var to = from + Duration.FromHours(1);

        var request = new HistoricalMarketDataRequest(symbols, from, to);
        symbols.Clear();

        Assert.Equal(from, request.From);
        Assert.Equal(to, request.To);
        Assert.Equal("MES.FUT", Assert.Single(request.Symbols));
        Assert.IsNotType<List<string>>(request.Symbols);
    }

    [Fact]
    public void HistoricalRangeMustEndAfterItStarts()
    {
        var instant = Instant.FromUtc(2026, 8, 5, 12, 0);

        Assert.Throws<ArgumentException>(() =>
            new HistoricalMarketDataRequest(["MES.FUT"], instant, instant));
    }

    [Fact]
    public void LiveRequestRequiresAtLeastOneSymbol()
    {
        Assert.Throws<ArgumentException>(() => new LiveMarketDataRequest([]));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(" MES.FUT")]
    [InlineData("MES.FUT ")]
    public void InvalidProviderSymbolIsRejected(string symbol)
    {
        Assert.Throws<ArgumentException>(() => new LiveMarketDataRequest([symbol]));
    }

    [Fact]
    public void DuplicateProviderSymbolsAreRejected()
    {
        Assert.Throws<ArgumentException>(() =>
            new LiveMarketDataRequest(["MES.FUT", "MES.FUT"]));
    }
}
