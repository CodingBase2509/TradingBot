using System.ComponentModel.DataAnnotations;

namespace Trading.Core.Config.Backtest;

public sealed class WriteOptions
{
    /// <summary>Output format (e.g., "parquet").</summary>
    public string Format { get; init; } = "parquet";

    /// <summary>Output directory (partitioned by symbol and year).</summary>
    [Required] 
    public string Dir { get; init; } = string.Empty;
}