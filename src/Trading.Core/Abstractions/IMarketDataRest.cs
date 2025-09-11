using Trading.Core.Domain;

namespace Trading.Core.Abstractions;

/// <summary>Reads historical market data via REST (e.g., klines / candles).</summary>
public interface IMarketDataRest
{
    Task<IReadOnlyList<Candle>> GetKlinesAsync( 
        string symbol,
        string interval, 
        DateTimeOffset startUtc, 
        DateTimeOffset endUtc, 
        CancellationToken ct = default);
}