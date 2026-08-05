using NodaTime;
using TradingPlatform.Market.Calendars;
using TradingPlatform.Market.Calendars.Config;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Calendars;

public sealed class MarketCalendarTests
{
    [Fact]
    public void RegularSessionUsesAHalfOpenTimeRange()
    {
        var calendar = TestMarketCalendars.Nyse();

        Assert.True(calendar.IsOpen(Instant.FromUtc(2026, 8, 3, 13, 30)));
        Assert.True(calendar.IsOpen(Instant.FromUtc(2026, 8, 3, 19, 59)));
        Assert.False(calendar.IsOpen(Instant.FromUtc(2026, 8, 3, 20, 0)));
    }

    [Fact]
    public void OvernightSessionBelongsToItsConfiguredTradingDate()
    {
        var calendar = CreateCalendar(
        [
            new MarketSessionConfig(
                IsoDayOfWeek.Sunday,
                new LocalTime(18, 0),
                new LocalTime(17, 0),
                MarketSessionType.Regular),
        ]);

        var session = calendar.GetSession(Instant.FromUtc(2026, 8, 3, 16, 0));

        Assert.NotNull(session);
        Assert.Equal(new LocalDate(2026, 8, 2), session.TradingDate);
    }

    [Fact]
    public void EmptyDateOverrideClosesARegularTradingDay()
    {
        var holiday = new LocalDate(2026, 12, 25);
        var calendar = CreateCalendar(
            CreateWeekdaySessions(),
            [new MarketCalendarDateOverrideConfig(holiday, [])]);

        Assert.False(calendar.IsTradingDay(holiday));
        Assert.Empty(calendar.GetSessions(holiday));
    }

    [Fact]
    public void DateOverrideReplacesRegularSessions()
    {
        var earlyClose = new LocalDate(2026, 12, 24);
        var calendar = CreateCalendar(
            CreateWeekdaySessions(),
            [
                new MarketCalendarDateOverrideConfig(
                    earlyClose,
                    [
                        new MarketSessionTimeConfig(
                            new LocalTime(9, 30),
                            new LocalTime(13, 0),
                            MarketSessionType.Regular),
                    ]),
            ]);

        var session = Assert.Single(calendar.GetSessions(earlyClose));

        Assert.Equal(Instant.FromUtc(2026, 12, 24, 18, 0), session.ClosesAt);
    }

    [Fact]
    public void SessionResolutionAccountsForDaylightSavingTime()
    {
        var calendar = CreateCalendar(
        [
            new MarketSessionConfig(
                IsoDayOfWeek.Sunday,
                new LocalTime(1, 30),
                new LocalTime(3, 30),
                MarketSessionType.Regular),
        ]);

        var session = Assert.Single(calendar.GetSessions(new LocalDate(2026, 3, 8)));

        Assert.Equal(Duration.FromHours(1), session.ClosesAt - session.OpensAt);
    }

    private static MarketCalendar CreateCalendar(
        IReadOnlyList<MarketSessionConfig> weeklySessions,
        IReadOnlyList<MarketCalendarDateOverrideConfig>? dateOverrides = null) =>
        MarketCalendarFactory.Create(
            new MarketCalendarConfigBuilder()
                .WithWeeklySessions(weeklySessions)
                .WithDateOverrides(dateOverrides ?? [])
                .Build());

    private static MarketSessionConfig[] CreateWeekdaySessions() =>
        Enumerable.Range((int)IsoDayOfWeek.Monday, 5)
            .Select(day => new MarketSessionConfig(
                (IsoDayOfWeek)day,
                new LocalTime(9, 30),
                new LocalTime(16, 0),
                MarketSessionType.Regular))
            .ToArray();
}
