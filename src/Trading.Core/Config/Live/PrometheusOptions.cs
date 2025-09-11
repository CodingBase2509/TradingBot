namespace Trading.Core.Config.Live;

public sealed class PrometheusOptions
{
    public bool Enabled { get; init; } = true;
    
    public int Port { get; init; } = 9464;
}