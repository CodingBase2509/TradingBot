using NodaTime;

namespace TradingPlatform.Market.DataSources;

internal sealed record HistoricalMarketDataRequest
{
    public HistoricalMarketDataRequest(
        IEnumerable<string> symbols,
        Instant from,
        Instant to)
    {
        if (to <= from)
        {
            throw new ArgumentException("The historical range must end after it starts.", nameof(to));
        }

        Symbols = MarketDataSourceRequestGuard.ValidateAndCopySymbols(symbols);
        From = from;
        To = to;
    }

    public IReadOnlyList<string> Symbols { get; }

    public Instant From { get; }

    public Instant To { get; }
}
