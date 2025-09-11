namespace Trading.Core.Config.Train;

/// <summary>Top-level options for the training application.</summary>
public sealed class TrainOptions
{
    public DataOptions Data { get; init; } = new();
    
    public ModelOutputOptions Model { get; init; } = new();
}