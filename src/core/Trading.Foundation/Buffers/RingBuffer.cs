using System;
using System.Runtime.CompilerServices;

namespace Trading.Foundation.Buffers;

/// <summary>
/// Fixed-size circular buffer for value types with overwrite-on-full semantics.
/// Optimized for single-writer / single-reader usage.
/// </summary>
/// <typeparam name="T">Value type stored in the buffer.</typeparam>
/// <param name="capacity">Fixed capacity; must be positive.</param>
/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="capacity"/> ≤ 0.</exception>
public class RingBuffer<T>(int capacity) where T : struct
{
    private readonly T[] items = capacity > 0
        ? new T[capacity]
        : throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");

    private int startIndex;
    private int count;

    /// <summary>Gets the fixed capacity (slot count).</summary>
    public int Capacity => items.Length;

    /// <summary>Gets the current number of logical items (≤ <see cref="Capacity"/>).</summary>
    public int Count => count;

    /// <summary>Gets whether the buffer has been completely filled at least once.</summary>
    public bool HasWarmup => count == Capacity;

    /// <summary>
    /// Appends a value; overwrites the oldest item when full (advances start cyclically).
    /// </summary>
    /// <param name="value">Value to append.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(in T value)
    {
        if (count < Capacity)
        {
            items[count++] = value;
            return;
        }

        items[startIndex] = value;
        startIndex = (startIndex + 1) % Capacity;
    }

    /// <summary>
    /// Copies the logical window (oldest→newest) into <paramref name="destination"/>.
    /// </summary>
    /// <param name="destination"> Target span; length must be greater than or equal to <c>Count</c>. </param>
    /// <param name="window">
    /// Out: read-only slice over <paramref name="destination"/> containing exactly <c>Count</c> items
    /// in logical order (oldest→newest). Empty if the buffer is empty.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="destination"/> is smaller than <c>Count</c>.
    /// </exception>
    public void CopyTo(Span<T> destination, out ReadOnlySpan<T> window)
    {
        if (count == 0)
        {
            window = ReadOnlySpan<T>.Empty;
            return;
        }

        if (destination.Length < count)
            throw new ArgumentException("Destination span is too small.", nameof(destination));

        // Not wrapped: items are contiguous from index 0
        if (count < Capacity)
        {
            items.AsSpan(0, count).CopyTo(destination);
            window = destination[..count];
            return;
        }

        // Wrapped: copy tail [startIndex..end) then head [0..startIndex)
        var tailLength = Capacity - startIndex;
        items.AsSpan(startIndex, tailLength).CopyTo(destination);          // first segment (oldest→end)
        items.AsSpan(0, startIndex).CopyTo(destination[tailLength..]);     // append wrap segment

        window = destination[..count];
    }


    /// <summary>
    /// Provides a zero-allocation view of the logical window as up to two contiguous segments.
    /// Iterating <paramref name="left"/> then <paramref name="right"/> yields the full sequence oldest→newest.
    /// </summary>
    /// <param name="left">Out: first contiguous slice starting at the logical start (oldest).</param>
    /// <param name="right">Out: second slice from array start up to (not including) the logical start.</param>
    public void GetSegments(out ReadOnlySpan<T> left, out ReadOnlySpan<T> right)
    {
        if (count == 0)
        {
            left = ReadOnlySpan<T>.Empty;
            right = ReadOnlySpan<T>.Empty;
            return;
        }

        if (count < Capacity)
        {
            left = items.AsSpan(0, count);
            right = ReadOnlySpan<T>.Empty;
            return;
        }

        var rightLength = Capacity - startIndex;
        left = items.AsSpan(startIndex, rightLength);
        right = items.AsSpan(0, startIndex);
    }
}
