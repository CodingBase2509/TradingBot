using TradingPlatform.Market.MarketData;

namespace TradingPlatform.Tests.Unit.Market.MarketData;

public sealed class MarketDataOriginTests
{
    [Fact]
    public void ValidOriginPreservesItsValues()
    {
        var origin = new MarketDataOrigin("Databento", 42);

        Assert.Equal("Databento", origin.Provider);
        Assert.Equal(42, origin.SequenceNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(" Databento")]
    [InlineData("Databento ")]
    public void InvalidProviderIsRejected(string? provider)
    {
        Assert.ThrowsAny<ArgumentException>(() => new MarketDataOrigin(provider!, 42));
    }

    [Fact]
    public void NegativeSequenceNumberIsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MarketDataOrigin("Databento", -1));
    }
}
