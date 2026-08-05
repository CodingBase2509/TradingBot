namespace TradingPlatform.Market.Calendars.Config;

internal sealed record MarketCalendarConfigIssue(
    MarketCalendarConfigIssueCode Code,
    string Message);
