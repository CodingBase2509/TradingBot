namespace Trading.Core.Config.Backtest;

public sealed class CostsOptions
{
    /// <summary>Exchange fee in basis points (bps).</summary>
    public int FeeBps { get; init; } = 7;

    /// <summary>Maximum allowed cost ratio (cost/ATR).</summary>
    public double MaxCostRatio { get; init; } = 0.15;
}