using NodaTime;
using TradingPlatform.Market.Calendars;
using TradingPlatform.Market.Calendars.Config;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Calendars.Config;

public sealed class MarketCalendarConfigValidatorTests
{
    [Fact]
    public void CompleteConfigIsValid()
    {
        var config = new MarketCalendarConfigBuilder().Build();

        var result = MarketCalendarConfigValidator.Validate(config);

        Assert.True(result.IsValid);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void IndependentConfigIssuesAreReturnedTogether()
    {
        var config = new MarketCalendarConfigBuilder()
            .WithMetadata(null)
            .WithCalendarId("invalid")
            .WithName("")
            .WithTimeZoneId("Unknown/Zone")
            .WithWeeklySessions([])
            .WithDateOverrides(null)
            .Build();

        var result = MarketCalendarConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.MissingMetadata);
        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.InvalidCalendarId);
        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.MissingName);
        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.UnknownTimeZone);
        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.MissingWeeklySessions);
        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.MissingDateOverrides);
    }

    [Fact]
    public void OverlappingWeeklySessionsAreRejected()
    {
        var config = new MarketCalendarConfigBuilder()
            .WithWeeklySessions(
            [
                new MarketSessionConfig(
                    IsoDayOfWeek.Monday,
                    new LocalTime(9, 0),
                    new LocalTime(12, 0),
                    MarketSessionType.Regular),
                new MarketSessionConfig(
                    IsoDayOfWeek.Monday,
                    new LocalTime(11, 0),
                    new LocalTime(13, 0),
                    MarketSessionType.Extended),
            ])
            .Build();

        var result = MarketCalendarConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.OverlappingSessions);
    }

    [Fact]
    public void OvernightSessionMayNotOverlapTheFollowingDay()
    {
        var config = new MarketCalendarConfigBuilder()
            .WithWeeklySessions(
            [
                new MarketSessionConfig(
                    IsoDayOfWeek.Sunday,
                    new LocalTime(18, 0),
                    new LocalTime(17, 0),
                    MarketSessionType.Regular),
                new MarketSessionConfig(
                    IsoDayOfWeek.Monday,
                    new LocalTime(16, 0),
                    new LocalTime(17, 30),
                    MarketSessionType.Extended),
            ])
            .Build();

        var result = MarketCalendarConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.OverlappingSessions);
    }

    [Fact]
    public void DuplicateAndOverlappingDateOverridesAreRejected()
    {
        var date = new LocalDate(2026, 12, 24);
        var config = new MarketCalendarConfigBuilder()
            .WithDateOverrides(
            [
                new MarketCalendarDateOverrideConfig(
                    date,
                    [
                        new MarketSessionTimeConfig(
                            new LocalTime(9, 30),
                            new LocalTime(13, 0),
                            MarketSessionType.Regular),
                        new MarketSessionTimeConfig(
                            new LocalTime(12, 0),
                            new LocalTime(14, 0),
                            MarketSessionType.Extended),
                    ]),
                new MarketCalendarDateOverrideConfig(date, []),
            ])
            .Build();

        var result = MarketCalendarConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.DuplicateDateOverride);
        Assert.Contains(result.Issues, issue => issue.Code is MarketCalendarConfigIssueCode.OverlappingSessions);
    }
}
