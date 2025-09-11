using Trading.Core.Domain;

namespace Trading.Core.Abstractions;

/// <summary>Streams candles (e.g., 15m klines) as they update and close.</summary>
public interface ICandleStream
{
    IAsyncEnumerable<Candle> StreamAsync(string symbol, string interval, CancellationToken ct = default);
}