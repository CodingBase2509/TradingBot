using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Trading.Core.Config.Live;

public sealed class LoggingOptions
{
    public LogLevel Level { get; init; }
    
    /// <summary>Sampling rate for high-volume logs (0..1).</summary>
    [Range(0, 1)] 
    public double SampleHighRate { get; init; } = 0.1;
}