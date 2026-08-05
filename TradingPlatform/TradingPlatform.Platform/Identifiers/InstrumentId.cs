namespace TradingPlatform.Platform.Identifiers;

public sealed record InstrumentId
{
    public InstrumentId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("Instrument IDs must not contain leading or trailing whitespace.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
