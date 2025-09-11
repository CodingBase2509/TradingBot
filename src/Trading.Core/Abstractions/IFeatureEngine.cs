using Trading.Core.Domain;
using Trading.Core.Features;

namespace Trading.Core.Abstractions;

/// <summary>Creates a feature row for a closed candle window (no learning, just feature extraction).</summary>
public interface IFeatureEngine
{
    FeatureRow Compute( ReadOnlySpan<Candle> window, OrderBookTick? latestBookTick, CancellationToken ct = default);
}