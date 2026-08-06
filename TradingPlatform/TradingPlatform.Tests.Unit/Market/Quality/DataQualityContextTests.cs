using NodaTime;
using TradingPlatform.Market.Quality;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Quality;

public sealed class DataQualityContextTests
{
    [Fact]
    public void CalendarMustBelongToInstrument()
    {
        Assert.Throws<ArgumentException>(() =>
            DataQualityContext.CreateHistorical(
                DataQualityTestData.Instrument(),
                TestMarketCalendars.Nyse()));
    }

    [Fact]
    public void LiveDataAgeMustBePositive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DataQualityContext.CreateLive(
                DataQualityTestData.Instrument(),
                DataQualityTestData.Calendar(),
                Duration.Zero,
                Duration.Zero));
    }

    [Fact]
    public void FutureSkewMustNotBeNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DataQualityContext.CreateLive(
                DataQualityTestData.Instrument(),
                DataQualityTestData.Calendar(),
                Duration.FromMinutes(1),
                Duration.FromSeconds(-1)));
    }
}
