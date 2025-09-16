namespace Trading.Tests.Core.Generators;

/// <summary>
/// Minimal generator contract for producing domain instances and series.
/// </summary>
/// <typeparam name="T">Model type to generate.</typeparam>
public interface IGenerator<T>
{
    /// <summary>Sets a deterministic seed.</summary>
    void SetSeed(int seed);

    /// <summary>Generates a single instance.</summary>
    T Generate();

    /// <summary>Generates a time-ordered series.</summary>
    List<T> GenerateSeries(int count, DateTimeOffset? startUtc = null, TimeSpan? step = null);
}