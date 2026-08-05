namespace TradingPlatform.Market.Instruments.Config;

internal sealed record InstrumentConfigValidationResult(
    IReadOnlyList<InstrumentConfigIssue> Issues)
{
    public bool IsValid => Issues.Count == 0;
}
