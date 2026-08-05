namespace TradingPlatform.Market.Calendars.Config;

internal enum MarketCalendarConfigIssueCode
{
    MissingMetadata,
    MissingCalendarId,
    InvalidCalendarId,
    MissingName,
    MissingTimeZone,
    UnknownTimeZone,
    MissingWeeklySessions,
    MissingDateOverrides,
    InvalidSession,
    InvalidDayOfWeek,
    UnknownSessionType,
    InvalidDateOverride,
    DuplicateDateOverride,
    OverlappingSessions,
}
