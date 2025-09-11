using System.ComponentModel.DataAnnotations;
using Trading.Core.Domain;

namespace Trading.Core.Config.Backtest;

/// <summary>Top-level options for backtesting and dataset generation.</summary>
public sealed class BacktestOptions
{
    [Required] 
    public string Exchange { get; init; } = "Binance";
    
    [Required] 
    public string[] Symbols { get; init; } = [];
    
    [Required] 
    public string Interval { get; init; } = Intervals.M15;

    [Required] 
    public BackfillOptions Backfill { get; init; } = new();
    
    public CostsOptions? Costs { get; init; }
}