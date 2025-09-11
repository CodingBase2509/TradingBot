namespace Trading.Core.Config.Live;

public sealed class TelemetryOptions
{
    public PrometheusOptions? Prometheus { get; init; }
    
    public LoggingOptions? Logging { get; init; }
}