using System.ComponentModel.DataAnnotations;
using Trading.Core.Domain;

namespace Trading.Core.Config.Live;

/// <summary>Top-level options for the live trading worker.</summary>
public sealed class LiveOptions
{
    [Required] 
    public string Exchange { get; init; } = string.Empty;
    
    [Required] 
    public string[] Symbols { get; init; } = [];
    
    [Required] 
    public string Interval { get; init; } = Intervals.M15;
    
    [Range(1, 180)] 
    public int LookbackDays { get; init; } = 45;

    public StreamsOptions? Streams { get; init; }
    
    public ModelOptions? Model { get; init; }
    
    public RiskOptions? Risk { get; init; }
    
    public TelemetryOptions? Telemetry { get; init; }
}