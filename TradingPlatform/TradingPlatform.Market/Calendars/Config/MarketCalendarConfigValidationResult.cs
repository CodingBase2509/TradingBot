namespace TradingPlatform.Market.Calendars.Config;

internal sealed record MarketCalendarConfigValidationResult(
    IReadOnlyList<MarketCalendarConfigIssue> Issues)
{
    public bool IsValid => Issues.Count == 0;
}
