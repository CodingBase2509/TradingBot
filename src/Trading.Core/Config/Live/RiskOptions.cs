using System.ComponentModel.DataAnnotations;

namespace Trading.Core.Config.Live;

public sealed class RiskOptions
{
    [Range(0, 1)] 
    public double RiskPct { get; init; } = 0.0075;
    
    [Range(0.1, 10)] 
    public double RrMinCosted { get; init; } = 1.5;
    
    [Range(0, 10)] 
    public double CostRatioMax { get; init; } = 0.15;
}