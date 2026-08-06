using NodaTime;
using TradingPlatform.Market.Quality;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Tests.Unit.Market.Quality;

public sealed class DataQualityTradeTests
{
    [Fact]
    public void ValidTradeIsAccepted()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        var trade = DataQualityTestData.Trade();

        var result = validator.Validate(trade);

        Assert.Equal(DataQualityDecision.Accepted, result.Decision);
        Assert.True(result.IsAccepted);
        Assert.Same(trade, result.Value);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void PriceAndQuantityMustMatchInstrumentSteps()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        var trade = DataQualityTestData.Trade(price: 5280.10m, quantity: 0.5m);

        var result = validator.Validate(trade);

        Assert.Equal(DataQualityDecision.Rejected, result.Decision);
        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.PriceNotOnTickSize);
        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.QuantityNotOnMinimumStep);
    }

    [Fact]
    public void TradeOutsideTradingSessionIsRejected()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        var saturday = Instant.FromUtc(2026, 8, 8, 14, 0);

        var result = validator.Validate(DataQualityTestData.Trade(occurredAt: saturday));

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.OutsideTradingSession);
        Assert.False(result.IsAccepted);
    }

    [Fact]
    public void InstrumentMustMatchQualityContext()
    {
        var validator = DataQualityTestData.HistoricalValidator();

        var result = validator.Validate(
            DataQualityTestData.Trade(instrumentId: new InstrumentId("NQ")));

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.InstrumentMismatch);
    }

    [Fact]
    public void ExactDuplicateIsRejected()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        var trade = DataQualityTestData.Trade();
        validator.Validate(trade);

        var result = validator.Validate(trade);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(DataQualityIssueCode.DuplicateEvent, issue.Code);
        Assert.Equal(DataQualityDecision.Rejected, result.Decision);
    }

    [Fact]
    public void SequenceGapIsAcceptedWithWarning()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        validator.Validate(DataQualityTestData.Trade(sequenceNumber: 10));

        var result = validator.Validate(DataQualityTestData.Trade(
            occurredAt: DataQualityTestData.DefaultOccurredAt + Duration.FromMilliseconds(1),
            sequenceNumber: 12));

        var issue = Assert.Single(result.Issues);
        Assert.Equal(DataQualityIssueCode.SequenceGap, issue.Code);
        Assert.Equal(DataQualityIssueSeverity.Warning, issue.Severity);
        Assert.Equal(DataQualityDecision.AcceptedWithWarnings, result.Decision);
        Assert.True(result.IsAccepted);
    }

    [Fact]
    public void ReusedSequenceNumberIsRejected()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        validator.Validate(DataQualityTestData.Trade(sequenceNumber: 10));

        var result = validator.Validate(DataQualityTestData.Trade(
            occurredAt: DataQualityTestData.DefaultOccurredAt + Duration.FromMilliseconds(1),
            price: 5280.50m,
            sequenceNumber: 10));

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.DuplicateSequenceNumber);
        Assert.False(result.IsAccepted);
    }

    [Fact]
    public void OlderSequenceNumberIsRejected()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        validator.Validate(DataQualityTestData.Trade(sequenceNumber: 10));

        var result = validator.Validate(DataQualityTestData.Trade(
            occurredAt: DataQualityTestData.DefaultOccurredAt + Duration.FromMilliseconds(1),
            sequenceNumber: 9));

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.SequenceOutOfOrder);
    }

    [Fact]
    public void OlderTimestampIsRejected()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        validator.Validate(DataQualityTestData.Trade());

        var result = validator.Validate(DataQualityTestData.Trade(
            occurredAt: DataQualityTestData.DefaultOccurredAt - Duration.FromMilliseconds(1),
            sequenceNumber: 11));

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.TimestampOutOfOrder);
    }

    [Fact]
    public void StaleLiveTradeIsRejected()
    {
        var validator = DataQualityTestData.LiveValidator();
        var trade = DataQualityTestData.Trade(
            occurredAt: DataQualityTestData.DefaultOccurredAt - Duration.FromMinutes(6));

        var result = validator.Validate(trade);

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.StaleLiveData);
    }

    [Fact]
    public void FutureLiveTradeBeyondToleranceIsRejected()
    {
        var validator = DataQualityTestData.LiveValidator();
        var trade = DataQualityTestData.Trade(
            occurredAt: DataQualityTestData.DefaultOccurredAt + Duration.FromMinutes(2));

        var result = validator.Validate(trade);

        Assert.Contains(result.Issues, issue => issue.Code is DataQualityIssueCode.TimestampTooFarInFuture);
    }

    [Fact]
    public void HistoricalTradeIsNotComparedWithCurrentTime()
    {
        var validator = DataQualityTestData.HistoricalValidator();
        var oldTradingDay = Instant.FromUtc(2020, 1, 6, 15, 0);

        var result = validator.Validate(DataQualityTestData.Trade(occurredAt: oldTradingDay));

        Assert.DoesNotContain(result.Issues, issue =>
            issue.Code is DataQualityIssueCode.StaleLiveData or
                DataQualityIssueCode.TimestampTooFarInFuture);
        Assert.True(result.IsAccepted);
    }
}
