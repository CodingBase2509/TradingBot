namespace TradingPlatform.Market.Normalization;

internal sealed record MarketDataNormalizationIssue(
    MarketDataNormalizationIssueCode Code,
    string Message);
