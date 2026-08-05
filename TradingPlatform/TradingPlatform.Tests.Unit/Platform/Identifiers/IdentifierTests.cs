using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Tests.Unit.Platform.Identifiers;

public sealed class IdentifierTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(" MES")]
    [InlineData("MES ")]
    public void InstrumentIdRejectsInvalidValues(string value)
    {
        Assert.ThrowsAny<ArgumentException>(() => new InstrumentId(value));
    }

    [Fact]
    public void StrategyInstanceIdCreatesANonEmptyUuidVersionSeven()
    {
        var strategyInstanceId = StrategyInstanceId.Create();

        Assert.NotEqual(Guid.Empty, strategyInstanceId.Value);
        Assert.Equal(7, strategyInstanceId.Value.Version);
    }
}
