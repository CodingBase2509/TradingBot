namespace TradingPlatform.Market.Instruments.Config;

internal sealed record InstrumentConfigIssue(
    InstrumentConfigIssueCode Code,
    string Message);
