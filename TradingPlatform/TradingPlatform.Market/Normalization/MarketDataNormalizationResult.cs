namespace TradingPlatform.Market.Normalization;

internal sealed record MarketDataNormalizationResult<T>
    where T : class
{
    private MarketDataNormalizationResult(T? value, IReadOnlyList<MarketDataNormalizationIssue> issues)
    {
        Value = value;
        Issues = issues;
    }

    public T? Value { get; }

    public IReadOnlyList<MarketDataNormalizationIssue> Issues { get; }

    public bool IsSuccess => Value is not null && Issues.Count == 0;

    public static MarketDataNormalizationResult<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new MarketDataNormalizationResult<T>(value, []);
    }

    public static MarketDataNormalizationResult<T> Failure(
        IEnumerable<MarketDataNormalizationIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);

        var snapshot = issues.ToArray();
        if (snapshot.Length == 0)
        {
            throw new ArgumentException("A failed normalization result requires at least one issue.", nameof(issues));
        }

        return new MarketDataNormalizationResult<T>(null, Array.AsReadOnly(snapshot));
    }
}
