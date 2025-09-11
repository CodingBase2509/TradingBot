namespace Trading.Core.Config.Live;

public sealed class ModelOptions
{
    public ModelSideOptions? Long { get; init; }
    
    public ModelSideOptions? Short { get; init; }
}