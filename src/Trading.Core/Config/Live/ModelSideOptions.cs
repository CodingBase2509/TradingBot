using System.ComponentModel.DataAnnotations;

namespace Trading.Core.Config.Live;

public sealed class ModelSideOptions
{
    [Required] 
    public string Path { get; init; } = string.Empty;
    
    [Range(0, 1)] 
    public double Threshold { get; init; }
}