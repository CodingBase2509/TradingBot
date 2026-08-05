using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Config;
using TradingPlatform.Platform.Config;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Instruments;

public sealed class InstrumentDefinitionFactoryTests
{
    [Fact]
    public void ActiveValidConfigCreatesACompleteDefinition()
    {
        var config = new InstrumentConfigBuilder().Build();

        var definition = InstrumentDefinitionFactory.CreateDefinition(config);

        Assert.Equal(config.InstrumentId, definition.Id);
        Assert.Equal(config.Name, definition.Name);
        Assert.Equal(config.InstrumentType, definition.Type);
        Assert.Equal(config.ProviderSymbols, definition.ProviderSymbols);
        Assert.Equal(TestInstruments.MesCalendarId, definition.CalendarId);
        Assert.Equal(TestInstruments.MesRolloverRuleId, definition.RolloverRuleId);
    }

    [Fact]
    public void InvalidConfigCannotCreateADefinition()
    {
        var config = new InstrumentConfigBuilder()
            .WithTradingValues(tickSize: 0, tickValue: 1, minimumQuantity: 1)
            .Build();

        var exception = Assert.Throws<ArgumentException>(() =>
            InstrumentDefinitionFactory.CreateDefinition(config));

        Assert.Contains(nameof(InstrumentConfigIssueCode.InvalidTickSize), exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(ConfigStatus.Draft)]
    [InlineData(ConfigStatus.Validated)]
    [InlineData(ConfigStatus.Superseded)]
    [InlineData(ConfigStatus.Retired)]
    public void NonActiveConfigCannotCreateARuntimeDefinition(ConfigStatus status)
    {
        var config = new InstrumentConfigBuilder()
            .WithStatus(status)
            .Build();

        Assert.Throws<InvalidOperationException>(() => InstrumentDefinitionFactory.CreateDefinition(config));
    }
}
