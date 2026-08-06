using NodaTime;
using TradingPlatform.Market.Quality;

namespace TradingPlatform.Tests.Unit.Market.Quality;

public sealed class DataQualityQuoteTests
{
    [Fact]
    public void ValidQuoteIsAccepted()
    {
        var validator = DataQualityTestData.HistoricalValidator();

        var result = validator.Validate(DataQualityTestData.Quote());

        Assert.Equal(DataQualityDecision.Accepted, result.Decision);
        Assert.True(result.IsAccepted);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void CrossedQuoteIsRejected()
    {
        var validator = DataQualityTestData.HistoricalValidator();

        var result = validator.Validate(DataQualityTestData.Quote(
            bidPrice: 5280.50m,
            askPrice: 5280.25m));

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.CrossedQuote);
        Assert.Equal(DataQualityDecision.Rejected, result.Decision);
    }

    [Fact]
    public void LockedQuoteIsAcceptedWithWarning()
    {
        var validator = DataQualityTestData.HistoricalValidator();

        var result = validator.Validate(DataQualityTestData.Quote(
            bidPrice: 5280.25m,
            askPrice: 5280.25m));

        var issue = Assert.Single(result.Issues);
        Assert.Equal(DataQualityIssueCode.LockedQuote, issue.Code);
        Assert.Equal(DataQualityIssueSeverity.Warning, issue.Severity);
        Assert.Equal(DataQualityDecision.AcceptedWithWarnings, result.Decision);
    }

    [Fact]
    public void QuotePricesAndQuantitiesMustMatchInstrumentSteps()
    {
        var validator = DataQualityTestData.HistoricalValidator();

        var result = validator.Validate(DataQualityTestData.Quote(
            bidPrice: 5280.10m,
            bidQuantity: 0.5m,
            askQuantity: 0.5m));

        Assert.Equal(DataQualityDecision.Rejected, result.Decision);
        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.PriceNotOnTickSize);
        Assert.Equal(
            2,
            result.Issues.Count(issue => issue.Code is DataQualityIssueCode.QuantityNotOnMinimumStep));
    }

    [Fact]
    public void TradeAndQuoteSequenceStateIsIndependent()
    {
        var validator = DataQualityTestData.HistoricalValidator();

        var tradeResult = validator.Validate(DataQualityTestData.Trade(sequenceNumber: 10));
        var quoteResult = validator.Validate(DataQualityTestData.Quote(sequenceNumber: 10));

        Assert.Equal(DataQualityDecision.Accepted, tradeResult.Decision);
        Assert.Equal(DataQualityDecision.Accepted, quoteResult.Decision);
    }

    [Fact]
    public void QuoteSequenceGapIsAcceptedWithWarning()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        validator.Validate(DataQualityTestData.Quote(sequenceNumber: 20));

        var result = validator.Validate(DataQualityTestData.Quote(
            occurredAt: DataQualityTestData.DefaultOccurredAt + Duration.FromMilliseconds(1),
            sequenceNumber: 22));

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.SequenceGap);
        Assert.Equal(DataQualityDecision.AcceptedWithWarnings, result.Decision);
    }
}
