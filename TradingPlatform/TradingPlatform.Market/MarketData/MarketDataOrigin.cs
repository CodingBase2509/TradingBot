namespace TradingPlatform.Market.MarketData;

public sealed record MarketDataOrigin
{
    internal MarketDataOrigin(string provider, long? sequenceNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);

        if (!string.Equals(provider, provider.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "The market data provider must not contain leading or trailing whitespace.",
                nameof(provider));
        }

        if (sequenceNumber < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sequenceNumber),
                sequenceNumber,
                "A market data sequence number must not be negative.");
        }

        Provider = provider;
        SequenceNumber = sequenceNumber;
    }

    public string Provider { get; }

    public long? SequenceNumber { get; }
}
