namespace Trading.Foundation.Config;

/// <summary>Compact, frequently-read decision parameters.</summary>
public record DecisionConfig(
    double ThresholdLong,
    double ThresholdShort,
    double MinimumRiskReward,
    double MaximumCostRatio,
    double RiskFractionPerTrade);