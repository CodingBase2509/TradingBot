namespace TradingPlatform.Market.DataSources;

internal sealed record LiveMarketDataRequest
{
    public LiveMarketDataRequest(IEnumerable<string> symbols)
    {
        Symbols = MarketDataSourceRequestGuard.ValidateAndCopySymbols(symbols);
    }

    public IReadOnlyList<string> Symbols { get; }
}
