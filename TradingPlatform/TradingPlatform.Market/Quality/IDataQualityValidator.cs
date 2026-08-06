using TradingPlatform.Market.MarketData;

namespace TradingPlatform.Market.Quality;

internal interface IDataQualityValidator
{
    DataQualityResult<Trade> Validate(Trade trade);

    DataQualityResult<Quote> Validate(Quote quote);
}
