namespace TradingPlatform.Market.DataSources;

internal interface IHistoricalMarketDataSource<out TRecord>
{
    IAsyncEnumerable<TRecord> ReadAsync(
        HistoricalMarketDataRequest request,
        CancellationToken cancellationToken = default);
}
