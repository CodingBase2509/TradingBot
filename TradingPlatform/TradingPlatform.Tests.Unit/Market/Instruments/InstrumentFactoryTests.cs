using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Config;
using TradingPlatform.Platform.Config;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Instruments;

public sealed class InstrumentFactoryTests
{
    [Fact]
    public void ActiveValidConfigCreatesACompleteInstrument()
    {
        var config = new InstrumentConfigBuilder().Build();

        var instrument = InstrumentFactory.Create(config);

        Assert.Equal(config.InstrumentId, instrument.Id);
        Assert.Equal(config.Name, instrument.Name);
        Assert.Equal(config.InstrumentType, instrument.Type);
        Assert.Equal(config.ProviderSymbols, instrument.ProviderSymbols);
        Assert.Equal(TestInstruments.MesCalendarId, instrument.CalendarId);
        Assert.Equal(TestInstruments.MesRolloverRuleId, instrument.RolloverRuleId);
    }

    [Fact]
    public void InvalidConfigCannotCreateAnInstrument()
    {
        var config = new InstrumentConfigBuilder()
            .WithTradingValues(tickSize: 0, tickValue: 1, minimumQuantity: 1)
            .Build();

        var exception = Assert.Throws<ArgumentException>(() =>
            InstrumentFactory.Create(config));

        Assert.Contains(nameof(InstrumentConfigIssueCode.InvalidTickSize), exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(ConfigStatus.Draft)]
    [InlineData(ConfigStatus.Validated)]
    [InlineData(ConfigStatus.Superseded)]
    [InlineData(ConfigStatus.Retired)]
    public void NonActiveConfigCannotCreateARuntimeInstrument(ConfigStatus status)
    {
        var config = new InstrumentConfigBuilder()
            .WithStatus(status)
            .Build();

        Assert.Throws<InvalidOperationException>(() => InstrumentFactory.Create(config));
    }
}
