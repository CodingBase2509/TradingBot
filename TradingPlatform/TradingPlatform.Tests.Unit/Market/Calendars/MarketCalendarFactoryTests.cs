using NodaTime;
using TradingPlatform.Market.Calendars;
using TradingPlatform.Market.Calendars.Config;
using TradingPlatform.Platform.Config;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Calendars;

public sealed class MarketCalendarFactoryTests
{
    [Fact]
    public void ActiveValidConfigCreatesACompleteCalendar()
    {
        var config = new MarketCalendarConfigBuilder().Build();

        var calendar = MarketCalendarFactory.Create(config);

        Assert.Equal(TestMarketCalendars.NyseId, calendar.Id);
        Assert.Equal(config.Name, calendar.Name);
        Assert.Equal(config.TimeZoneId, calendar.TimeZone.Id);
        Assert.True(calendar.IsTradingDay(new LocalDate(2026, 8, 3)));
    }

    [Fact]
    public void InvalidConfigCannotCreateACalendar()
    {
        var config = new MarketCalendarConfigBuilder()
            .WithTimeZoneId("Unknown/Zone")
            .Build();

        var exception = Assert.Throws<ArgumentException>(() => MarketCalendarFactory.Create(config));

        Assert.Contains(nameof(MarketCalendarConfigIssueCode.UnknownTimeZone), exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(ConfigStatus.Draft)]
    [InlineData(ConfigStatus.Validated)]
    [InlineData(ConfigStatus.Superseded)]
    [InlineData(ConfigStatus.Retired)]
    public void NonActiveConfigCannotCreateARuntimeCalendar(ConfigStatus status)
    {
        var config = new MarketCalendarConfigBuilder()
            .WithStatus(status)
            .Build();

        Assert.Throws<InvalidOperationException>(() => MarketCalendarFactory.Create(config));
    }
}
