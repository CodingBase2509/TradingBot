namespace Trading.Core.Config.Live;

public sealed class StreamsOptions
{
    public bool Kline { get; init; }
    
    public bool BookTicker { get; init; }
    
    public ReplayFallbackOptions? ReplayFallback { get; init; }
}