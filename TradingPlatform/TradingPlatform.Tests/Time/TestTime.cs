using Microsoft.Extensions.Time.Testing;

namespace TradingPlatform.Tests.Time;

public static class TestTime
{
    public static FakeTimeProvider At(DateTimeOffset utcNow)
    {
        if (utcNow.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Test time must use a UTC offset.", nameof(utcNow));
        }

        return new FakeTimeProvider(utcNow);
    }
}
