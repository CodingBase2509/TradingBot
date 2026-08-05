using NodaTime;
using TradingPlatform.Market.Calendars.Config;
using TradingPlatform.Platform.Config;

namespace TradingPlatform.Market.Calendars;

internal static class MarketCalendarFactory
{
    public static MarketCalendar Create(MarketCalendarConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var validationResult = MarketCalendarConfigValidator.Validate(config);
        if (!validationResult.IsValid)
        {
            var issueCodes = string.Join(", ", validationResult.Issues.Select(issue => issue.Code));
            throw new ArgumentException($"The market calendar config is invalid: {issueCodes}.", nameof(config));
        }

        if (config.Metadata.Status is not ConfigStatus.Active)
        {
            throw new InvalidOperationException("Only an active market calendar config can create a runtime calendar.");
        }

        var weeklySessions = config.WeeklySessions.Select(session =>
            new WeeklyMarketSessionSchedule(
                session.DayOfWeek,
                new MarketSessionSchedule(session.OpensAt, session.ClosesAt, session.Type)));
        var dateOverrides = config.DateOverrides.Select(dateOverride =>
            new MarketCalendarDateOverride(
                dateOverride.Date,
                dateOverride.Sessions
                    .Select(session => new MarketSessionSchedule(session.OpensAt, session.ClosesAt, session.Type))
                    .ToArray()));

        return new MarketCalendar(
            Guid.Parse(config.CalendarId),
            config.Name,
            DateTimeZoneProviders.Tzdb[config.TimeZoneId],
            weeklySessions,
            dateOverrides);
    }
}
