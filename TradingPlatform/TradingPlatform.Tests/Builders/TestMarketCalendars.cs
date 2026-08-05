using TradingPlatform.Market.Calendars;

namespace TradingPlatform.Tests.Builders;

public static class TestMarketCalendars
{
    public static Guid NyseId { get; } = Guid.Parse("44444444-4444-7444-8444-444444444444");

    public static MarketCalendar Nyse() => MarketCalendarFactory.Create(
        new MarketCalendarConfigBuilder().Build());
}
