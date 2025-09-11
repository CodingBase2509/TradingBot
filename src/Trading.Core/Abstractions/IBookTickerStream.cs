using Trading.Core.Domain;

namespace Trading.Core.Abstractions;

/// <summary>Streams best bid/ask updates (book ticker) for live cost/spread estimation.</summary>
public interface IBookTickerStream
{
    IAsyncEnumerable<OrderBookTick> StreamAsync(string symbol, CancellationToken ct = default);
}