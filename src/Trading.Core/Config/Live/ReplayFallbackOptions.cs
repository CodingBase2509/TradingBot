namespace Trading.Core.Config.Live;

public sealed class ReplayFallbackOptions
{
    public bool Enabled { get; init; }
    
    public string? Dir { get; init; }
}