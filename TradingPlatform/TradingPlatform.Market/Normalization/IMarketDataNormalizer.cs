using TradingPlatform.Market.MarketData;

namespace TradingPlatform.Market.Normalization;

internal interface IMarketDataNormalizer
{
    ValueTask<MarketDataNormalizationResult<Trade>> NormalizeAsync(
        TradeNormalizationInput input,
        CancellationToken cancellationToken = default);

    ValueTask<MarketDataNormalizationResult<Quote>> NormalizeAsync(
        QuoteNormalizationInput input,
        CancellationToken cancellationToken = default);
}
