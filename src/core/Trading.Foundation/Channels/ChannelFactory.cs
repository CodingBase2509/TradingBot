using System;
using System.Threading.Channels;

namespace Trading.Foundation.Channels;

/// <summary>
/// Factory for creating channels with sensible defaults for our pipelines.
/// </summary>
public static class ChannelFactory
{
    /// <summary>
    /// Creates a bounded channel with configurable writer/reader settings.
    /// </summary>
    /// <typeparam name="T">Item type transported through the channel.</typeparam>
    /// <param name="capacity">Fixed capacity; must be positive.</param>
    /// <param name="fullMode">
    /// Behavior when the channel is full. Default is <see cref="BoundedChannelFullMode.Wait"/>.
    /// </param>
    /// <param name="singleWriter">Optimizes for a single producer if <c>true</c>.</param>
    /// <param name="singleReader">Optimizes for a single consumer if <c>true</c>.</param>
    /// <param name="allowSynchronousContinuations">
    /// If <c>true</c>, continuations may run synchronously (reduced scheduling overhead).
    /// </param>
    /// <returns>A configured bounded channel instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="capacity"/> ≤ 0.</exception>
    public static Channel<T> CreateBounded<T>(
        int capacity,
        BoundedChannelFullMode fullMode = BoundedChannelFullMode.Wait,
        bool singleWriter = true,
        bool singleReader = true,
        bool allowSynchronousContinuations = false)
    {
        if (capacity <= 0) 
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");

        var options = new BoundedChannelOptions(capacity)
        {
            SingleWriter = singleWriter,
            SingleReader = singleReader,
            FullMode = fullMode,
            AllowSynchronousContinuations = allowSynchronousContinuations
        };
        return Channel.CreateBounded<T>(options);
    }

    /// <summary>
    /// Creates an unbounded channel with configurable writer/reader settings.
    /// </summary>
    /// <typeparam name="T">Item type transported through the channel.</typeparam>
    /// <param name="singleWriter">Optimizes for a single producer if <c>true</c>.</param>
    /// <param name="singleReader">Optimizes for a single consumer if <c>true</c>.</param>
    /// <param name="allowSynchronousContinuations">
    /// If <c>true</c>, continuations may run synchronously (lower overhead, use carefully).
    /// </param>
    /// <returns>A configured unbounded channel instance.</returns>
    public static Channel<T> CreateUnbounded<T>(
        bool singleWriter = true,
        bool singleReader = true,
        bool allowSynchronousContinuations = false)
    {
        var options = new UnboundedChannelOptions
        {
            SingleWriter = singleWriter,
            SingleReader = singleReader,
            AllowSynchronousContinuations = allowSynchronousContinuations
        };
        return Channel.CreateUnbounded<T>(options);
    }
}
