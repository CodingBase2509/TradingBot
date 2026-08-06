namespace TradingPlatform.Market.Quality;

internal sealed record DataQualityResult<T>
    where T : class
{
    private DataQualityResult(
        T value,
        DataQualityDecision decision,
        IReadOnlyList<DataQualityIssue> issues)
    {
        Value = value;
        Decision = decision;
        Issues = issues;
    }

    public T Value { get; }

    public DataQualityDecision Decision { get; }

    public IReadOnlyList<DataQualityIssue> Issues { get; }

    public bool IsAccepted => Decision is not DataQualityDecision.Rejected;

    public static DataQualityResult<T> Create(T value, IEnumerable<DataQualityIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(issues);

        var snapshot = issues.ToArray();
        var decision = snapshot.Any(issue => issue.Severity is DataQualityIssueSeverity.Error)
            ? DataQualityDecision.Rejected
            : snapshot.Length > 0
                ? DataQualityDecision.AcceptedWithWarnings
                : DataQualityDecision.Accepted;

        return new DataQualityResult<T>(value, decision, Array.AsReadOnly(snapshot));
    }
}
