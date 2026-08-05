namespace TradingPlatform.Market.DataSources;

internal interface ILiveMarketDataSource<out TRecord>
{
    IAsyncEnumerable<TRecord> SubscribeAsync(
        LiveMarketDataRequest request,
        CancellationToken cancellationToken = default);
}
