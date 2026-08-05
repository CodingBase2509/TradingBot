using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Identifiers;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Instruments;

public sealed class InstrumentDefinitionTests
{
    [Fact]
    public void MesProfileContainsTheExpectedTradingValues()
    {
        var instrument = TestInstruments.Mes();

        Assert.Equal(new InstrumentId("MES"), instrument.Id);
        Assert.Equal(0.25m, instrument.TickSize);
        Assert.Equal(1.25m, instrument.TickValue);
        Assert.Equal(1m, instrument.MinimumQuantity);
        Assert.True(instrument.Capabilities.RequiresRollover);
    }

    [Fact]
    public void RolloverRequiresExpiringContracts()
    {
        var capabilities = new InstrumentCapabilities(
            SupportsLong: true,
            SupportsShort: true,
            HasExpiringContracts: false,
            RequiresRollover: true);

        var exception = Assert.Throws<ArgumentException>(() => new InstrumentDefinition(
            new InstrumentId("TEST"),
            "Test instrument",
            InstrumentType.Future,
            "TEST",
            "USD",
            tickSize: 1m,
            tickValue: 1m,
            minimumQuantity: 1m,
            capabilities,
            [new ProviderSymbol(ProviderKind.MarketData, "TestProvider", "TEST")],
            Guid.NewGuid(),
            Guid.NewGuid()));

        Assert.Equal("capabilities", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.25)]
    public void TickSizeMustBePositive(decimal tickSize)
    {
        var mes = TestInstruments.Mes();

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new InstrumentDefinition(
            mes.Id,
            mes.Name,
            mes.Type,
            mes.Exchange,
            mes.Currency,
            tickSize,
            mes.TickValue,
            mes.MinimumQuantity,
            mes.Capabilities,
            mes.ProviderSymbols,
            mes.CalendarId,
            mes.RolloverRuleId));

        Assert.Equal("tickSize", exception.ParamName);
    }
}
