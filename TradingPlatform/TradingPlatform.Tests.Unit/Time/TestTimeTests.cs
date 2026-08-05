using TradingPlatform.Tests.Time;

namespace TradingPlatform.Tests.Unit.Time;

public sealed class TestTimeTests
{
    [Fact]
    public void TimeCanAdvanceWithoutWaiting()
    {
        var start = new DateTimeOffset(2026, 8, 4, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = TestTime.At(start);

        timeProvider.Advance(TimeSpan.FromMinutes(5));

        Assert.Equal(start.AddMinutes(5), timeProvider.GetUtcNow());
    }
}
