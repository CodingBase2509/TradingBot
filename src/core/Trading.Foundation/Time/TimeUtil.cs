using System;
using System.Collections.Generic;
using Trading.Core.Domain;

namespace Trading.Foundation.Time;

/// <summary>
/// Time utilities used across the app (assumes UTC timestamps).
/// </summary>
public static class TimeUtil
{
    /// <summary>
    /// Converts a UTC timestamp to Unix epoch milliseconds.
    /// </summary>
    /// <param name="timestampUtc">UTC timestamp.</param>
    /// <returns>Unix time in milliseconds.</returns>
    public static long ToUnixMilliseconds(this DateTimeOffset timestampUtc) =>
        timestampUtc.ToUnixTimeMilliseconds();

    /// <summary>
    /// Converts Unix epoch milliseconds to a UTC timestamp.
    /// </summary>
    /// <param name="unixMilliseconds">Unix time in milliseconds.</param>
    /// <returns>UTC timestamp.</returns>
    public static DateTimeOffset FromUnixMilliseconds(long unixMilliseconds) =>
        DateTimeOffset.FromUnixTimeMilliseconds(unixMilliseconds);

    /// <summary>
    /// Returns the next 15-minute bar close (00, 15, 30, 45) strictly after <paramref name="timestampUtc"/>.
    /// </summary>
    /// <param name="timestampUtc">UTC timestamp.</param>
    /// <returns>Next 15-minute boundary.</returns>
    public static DateTimeOffset NextM15Close(DateTimeOffset timestampUtc)
    {
        // Floor to current 15-min bucket, then advance by 15 minutes.
        var minuteBucket = (timestampUtc.Minute / 15) * 15;
        var floor = new DateTimeOffset(
            timestampUtc.Year, timestampUtc.Month, timestampUtc.Day,
            timestampUtc.Hour, minuteBucket, 0, TimeSpan.Zero);

        return floor.AddMinutes(15);
    }

    /// <summary>
    /// Splits a UTC time range into contiguous chunks of fixed size in hours.
    /// </summary>
    /// <param name="startUtc">Inclusive range start.</param>
    /// <param name="endUtc">Inclusive range end (if equal to start, yields no chunks).</param>
    /// <param name="chunkSizeHours">Chunk size in hours (must be > 0).</param>
    /// <returns>Sequence of time ranges covering [start..end].</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="chunkSizeHours"/> ≤ 0.</exception>
    public static IEnumerable<TimeRangeUtc> ChunkByHours(
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        int chunkSizeHours)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSizeHours);
        
        if (endUtc <= startUtc) 
            yield break;

        var current = startUtc;
        while (current < endUtc)
        {
            var next = current.AddHours(chunkSizeHours);
            yield return new TimeRangeUtc(current, next <= endUtc ? next : endUtc);
            current = next;
        }
    }
}
