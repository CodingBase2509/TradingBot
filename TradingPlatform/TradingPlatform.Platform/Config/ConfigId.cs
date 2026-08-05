namespace TradingPlatform.Platform.Config;

public sealed record ConfigId
{
    public ConfigId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A config ID must not be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public static ConfigId Create() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
