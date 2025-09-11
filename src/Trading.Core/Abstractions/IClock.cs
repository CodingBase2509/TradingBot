namespace Trading.Core.Abstractions;

/// <summary>Provides the current UTC time.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}