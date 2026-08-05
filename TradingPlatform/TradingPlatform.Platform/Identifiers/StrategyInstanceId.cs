namespace TradingPlatform.Platform.Identifiers;

public sealed record StrategyInstanceId
{
    public StrategyInstanceId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A strategy instance ID must not be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public static StrategyInstanceId Create() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
