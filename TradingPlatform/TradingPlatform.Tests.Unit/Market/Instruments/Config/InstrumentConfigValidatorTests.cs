using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Config;
using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Instruments.Config;

public sealed class InstrumentConfigValidatorTests
{
    [Fact]
    public void CompleteMesConfigIsValid()
    {
        var config = new InstrumentConfigBuilder().Build();

        var result = InstrumentConfigValidator.Validate(config);

        Assert.True(result.IsValid);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void AllIndependentIssuesAreReturnedTogether()
    {
        var config = new InstrumentConfigBuilder()
            .WithName(string.Empty)
            .WithInstrumentType(InstrumentType.Unknown)
            .WithExchange(string.Empty)
            .WithCurrency(string.Empty)
            .WithTradingValues(tickSize: 0, tickValue: -1, minimumQuantity: 0)
            .WithCapabilities(new InstrumentCapabilities(false, false, false, false))
            .WithProviderSymbols([])
            .WithCalendarId(null)
            .WithRolloverRuleId(null)
            .Build();

        var result = InstrumentConfigValidator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingName);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.UnknownInstrumentType);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingExchange);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingCurrency);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.InvalidTickSize);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.InvalidTickValue);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.InvalidMinimumQuantity);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.UnsupportedTradeDirection);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingProviderSymbols);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingMarketDataSymbol);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingCalendar);
    }

    [Fact]
    public void ProviderEntriesAreValidatedAndMustBeUnique()
    {
        ProviderSymbol[] providerSymbols =
        [
            new ProviderSymbol(ProviderKind.Unknown, string.Empty, string.Empty),
            new ProviderSymbol(ProviderKind.Broker, "InteractiveBrokers", "MES"),
            new ProviderSymbol(ProviderKind.Broker, "interactivebrokers", "MES-duplicate"),
        ];
        var config = new InstrumentConfigBuilder()
            .WithProviderSymbols(providerSymbols)
            .Build();

        var result = InstrumentConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.UnknownProviderKind);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingProviderName);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingProviderSymbol);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.DuplicateProviderSymbol);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingMarketDataSymbol);
    }

    [Theory]
    [InlineData(null, nameof(InstrumentConfigIssueCode.MissingCalendar))]
    [InlineData("", nameof(InstrumentConfigIssueCode.MissingCalendar))]
    [InlineData("not-a-guid", nameof(InstrumentConfigIssueCode.InvalidCalendarId))]
    [InlineData("00000000-0000-0000-0000-000000000000", nameof(InstrumentConfigIssueCode.InvalidCalendarId))]
    public void CalendarIdMustBePresentAndValid(string? calendarId, string expectedCode)
    {
        var config = new InstrumentConfigBuilder()
            .WithCalendarId(calendarId)
            .Build();

        var result = InstrumentConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code.ToString() == expectedCode);
    }

    [Fact]
    public void RolloverRequiresExpiringContractsAndARule()
    {
        var config = new InstrumentConfigBuilder()
            .WithCapabilities(new InstrumentCapabilities(true, true, false, true))
            .WithRolloverRuleId(null)
            .Build();

        var result = InstrumentConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.RolloverWithoutExpiringContracts);
        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.MissingRolloverRule);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void RolloverRuleIdMustBeAValidNonEmptyGuid(string rolloverRuleId)
    {
        var config = new InstrumentConfigBuilder()
            .WithRolloverRuleId(rolloverRuleId)
            .Build();

        var result = InstrumentConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.InvalidRolloverRuleId);
    }

    [Fact]
    public void RolloverRuleIsRejectedWhenRolloverIsDisabled()
    {
        var config = new InstrumentConfigBuilder()
            .WithCapabilities(new InstrumentCapabilities(true, true, false, false))
            .Build();

        var result = InstrumentConfigValidator.Validate(config);

        Assert.Contains(result.Issues, issue => issue.Code is InstrumentConfigIssueCode.RolloverRuleWithoutRollover);
    }
}
