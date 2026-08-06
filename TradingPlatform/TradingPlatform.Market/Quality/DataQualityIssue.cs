namespace TradingPlatform.Market.Quality;

internal sealed record DataQualityIssue(
    DataQualityIssueCode Code,
    DataQualityIssueSeverity Severity,
    string Message);
