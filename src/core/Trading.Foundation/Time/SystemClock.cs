using Trading.Core.Abstractions;

namespace Trading.Foundation.Time;

/// <summary>Default system clock based on <see cref="DateTimeOffset.UtcNow"/>.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}