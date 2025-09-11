using System.ComponentModel.DataAnnotations;

namespace Trading.Core.Config.Backtest;

public sealed class BackfillOptions
{
    [Required] 
    public DateTimeOffset StartUtc { get; init; }
    
    [Required] 
    public DateTimeOffset EndUtc { get; init; }

    /// <summary>Batch size for REST requests (in hours), e.g., 720h ≈ 30 days.</summary>
    [Range(1, 24 * 31)] 
    public int BatchHours { get; init; }

    public WriteOptions Write { get; init; } = new();
}