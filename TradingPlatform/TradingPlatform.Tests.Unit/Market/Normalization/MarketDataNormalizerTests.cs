using NodaTime;
using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Catalog;
using TradingPlatform.Market.Normalization;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Normalization;

public sealed class MarketDataNormalizerTests
{
    [Fact]
    public async Task ValidTradeInputCreatesACanonicalTrade()
    {
        var instrument = TestInstruments.Mes();
        var normalizer = CreateNormalizer(instrument);
        var occurredAt = Instant.FromUtc(2026, 8, 5, 12, 0);
        var input = new TradeNormalizationInput(
            "Databento",
            "MES.FUT",
            occurredAt,
            5280.25m,
            3m,
            42);

        var result = await normalizer.NormalizeAsync(input);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Issues);
        Assert.NotNull(result.Value);
        Assert.Equal(instrument.Id, result.Value.InstrumentId);
        Assert.Equal(occurredAt, result.Value.OccurredAt);
        Assert.Equal(5280.25m, result.Value.Price);
        Assert.Equal(3m, result.Value.Quantity);
        Assert.Equal("Databento", result.Value.Origin.Provider);
        Assert.Equal(42, result.Value.Origin.SequenceNumber);
    }

    [Fact]
    public async Task ValidQuoteInputCreatesACanonicalQuote()
    {
        var instrument = TestInstruments.Mes();
        var normalizer = CreateNormalizer(instrument);
        var input = new QuoteNormalizationInput(
            "Databento",
            "MES.FUT",
            Instant.FromUtc(2026, 8, 5, 12, 0),
            5280.00m,
            12m,
            5280.25m,
            8m,
            43);

        var result = await normalizer.NormalizeAsync(input);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(instrument.Id, result.Value.InstrumentId);
        Assert.Equal(5280.00m, result.Value.BidPrice);
        Assert.Equal(5280.25m, result.Value.AskPrice);
        Assert.Equal(43, result.Value.Origin.SequenceNumber);
    }

    [Fact]
    public async Task IndependentTradeInputIssuesAreReturnedTogether()
    {
        var normalizer = CreateNormalizer(TestInstruments.Mes());
        var input = new TradeNormalizationInput(
            null,
            " MES.FUT ",
            Instant.FromUtc(2026, 8, 5, 12, 0),
            Price: 0,
            Quantity: -1,
            SequenceNumber: -1);

        var result = await normalizer.NormalizeAsync(input);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.MissingProvider);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidSymbol);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidTradePrice);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidTradeQuantity);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidSequenceNumber);
    }

    [Fact]
    public async Task IndependentQuoteValueIssuesAreReturnedTogether()
    {
        var normalizer = CreateNormalizer(TestInstruments.Mes());
        var input = new QuoteNormalizationInput(
            "Databento",
            "MES.FUT",
            Instant.FromUtc(2026, 8, 5, 12, 0),
            BidPrice: 0,
            BidQuantity: -1,
            AskPrice: 0,
            AskQuantity: -1);

        var result = await normalizer.NormalizeAsync(input);

        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidBidPrice);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidBidQuantity);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidAskPrice);
        Assert.Contains(result.Issues, issue => issue.Code is MarketDataNormalizationIssueCode.InvalidAskQuantity);
    }

    [Fact]
    public async Task UnknownProviderSymbolReturnsAControlledIssue()
    {
        var normalizer = CreateNormalizer(TestInstruments.Mes());
        var input = new TradeNormalizationInput(
            "Databento",
            "UNKNOWN",
            Instant.FromUtc(2026, 8, 5, 12, 0),
            1m,
            1m);

        var result = await normalizer.NormalizeAsync(input);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(MarketDataNormalizationIssueCode.UnknownInstrument, issue.Code);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task CrossedQuoteIsLeftForTheQualityLayer()
    {
        var normalizer = CreateNormalizer(TestInstruments.Mes());
        var input = new QuoteNormalizationInput(
            "Databento",
            "MES.FUT",
            Instant.FromUtc(2026, 8, 5, 12, 0),
            BidPrice: 5280.50m,
            BidQuantity: 1m,
            AskPrice: 5280.25m,
            AskQuantity: 1m);

        var result = await normalizer.NormalizeAsync(input);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.BidPrice > result.Value.AskPrice);
    }

    [Fact]
    public async Task CancellationIsHonoredBeforeNormalization()
    {
        var normalizer = CreateNormalizer(TestInstruments.Mes());
        var input = new TradeNormalizationInput(
            "Databento",
            "MES.FUT",
            Instant.FromUtc(2026, 8, 5, 12, 0),
            1m,
            1m);
        var cancellationToken = new CancellationToken(canceled: true);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => normalizer.NormalizeAsync(input, cancellationToken).AsTask());
    }

    private static MarketDataNormalizer CreateNormalizer(params Instrument[] instruments) =>
        new(new InMemoryInstrumentCatalog(instruments));
}
