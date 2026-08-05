using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Catalog;
using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Market.MarketData;

namespace TradingPlatform.Market.Normalization;

internal sealed class MarketDataNormalizer(IInstrumentCatalog instrumentCatalog) : IMarketDataNormalizer
{
    private readonly IInstrumentCatalog _instrumentCatalog =
        instrumentCatalog ?? throw new ArgumentNullException(nameof(instrumentCatalog));

    public async ValueTask<MarketDataNormalizationResult<Trade>> NormalizeAsync(
        TradeNormalizationInput input,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        cancellationToken.ThrowIfCancellationRequested();

        var issues = ValidateSource(input.Provider, input.Symbol, input.SequenceNumber);
        AddNonPositiveIssue(
            issues,
            input.Price,
            MarketDataNormalizationIssueCode.InvalidTradePrice,
            "A trade price must be greater than zero.");
        AddNonPositiveIssue(
            issues,
            input.Quantity,
            MarketDataNormalizationIssueCode.InvalidTradeQuantity,
            "A trade quantity must be greater than zero.");

        if (issues.Count > 0)
        {
            return MarketDataNormalizationResult<Trade>.Failure(issues);
        }

        var instrument = await FindInstrumentAsync(
            input.Provider!,
            input.Symbol!,
            cancellationToken);
        if (instrument is null)
        {
            return MarketDataNormalizationResult<Trade>.Failure(
            [
                new MarketDataNormalizationIssue(
                    MarketDataNormalizationIssueCode.UnknownInstrument,
                    $"No instrument is configured for '{input.Provider}:{input.Symbol}'."),
            ]);
        }

        var origin = new MarketDataOrigin(input.Provider!, input.SequenceNumber);
        var trade = new Trade(instrument.Id, input.OccurredAt, input.Price, input.Quantity, origin);
        return MarketDataNormalizationResult<Trade>.Success(trade);
    }

    public async ValueTask<MarketDataNormalizationResult<Quote>> NormalizeAsync(
        QuoteNormalizationInput input,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        cancellationToken.ThrowIfCancellationRequested();

        var issues = ValidateSource(input.Provider, input.Symbol, input.SequenceNumber);
        AddNonPositiveIssue(
            issues,
            input.BidPrice,
            MarketDataNormalizationIssueCode.InvalidBidPrice,
            "A bid price must be greater than zero.");
        AddNonPositiveIssue(
            issues,
            input.BidQuantity,
            MarketDataNormalizationIssueCode.InvalidBidQuantity,
            "A bid quantity must be greater than zero.");
        AddNonPositiveIssue(
            issues,
            input.AskPrice,
            MarketDataNormalizationIssueCode.InvalidAskPrice,
            "An ask price must be greater than zero.");
        AddNonPositiveIssue(
            issues,
            input.AskQuantity,
            MarketDataNormalizationIssueCode.InvalidAskQuantity,
            "An ask quantity must be greater than zero.");

        if (issues.Count > 0)
        {
            return MarketDataNormalizationResult<Quote>.Failure(issues);
        }

        var instrument = await FindInstrumentAsync(
            input.Provider!,
            input.Symbol!,
            cancellationToken);
        if (instrument is null)
        {
            return MarketDataNormalizationResult<Quote>.Failure(
            [
                new MarketDataNormalizationIssue(
                    MarketDataNormalizationIssueCode.UnknownInstrument,
                    $"No instrument is configured for '{input.Provider}:{input.Symbol}'."),
            ]);
        }

        var origin = new MarketDataOrigin(input.Provider!, input.SequenceNumber);
        var quote = new Quote(
            instrument.Id,
            input.OccurredAt,
            input.BidPrice,
            input.BidQuantity,
            input.AskPrice,
            input.AskQuantity,
            origin);
        return MarketDataNormalizationResult<Quote>.Success(quote);
    }

    private async ValueTask<Instrument?> FindInstrumentAsync(
        string provider,
        string symbol,
        CancellationToken cancellationToken)
    {
        var providerSymbol = new ProviderSymbol(ProviderKind.MarketData, provider, symbol);
        return await _instrumentCatalog.GetAsync(providerSymbol, cancellationToken);
    }

    private static List<MarketDataNormalizationIssue> ValidateSource(
        string? provider,
        string? symbol,
        long? sequenceNumber)
    {
        var issues = new List<MarketDataNormalizationIssue>();
        ValidateRequiredValue(
            issues,
            provider,
            MarketDataNormalizationIssueCode.MissingProvider,
            MarketDataNormalizationIssueCode.InvalidProvider,
            "A market data provider is required.",
            "The market data provider must not contain leading or trailing whitespace.");
        ValidateRequiredValue(
            issues,
            symbol,
            MarketDataNormalizationIssueCode.MissingSymbol,
            MarketDataNormalizationIssueCode.InvalidSymbol,
            "A provider symbol is required.",
            "The provider symbol must not contain leading or trailing whitespace.");

        if (sequenceNumber < 0)
        {
            issues.Add(new MarketDataNormalizationIssue(
                MarketDataNormalizationIssueCode.InvalidSequenceNumber,
                "A market data sequence number must not be negative."));
        }

        return issues;
    }

    private static void ValidateRequiredValue(
        List<MarketDataNormalizationIssue> issues,
        string? value,
        MarketDataNormalizationIssueCode missingCode,
        MarketDataNormalizationIssueCode invalidCode,
        string missingMessage,
        string invalidMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            issues.Add(new MarketDataNormalizationIssue(missingCode, missingMessage));
        }
        else if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            issues.Add(new MarketDataNormalizationIssue(invalidCode, invalidMessage));
        }
    }

    private static void AddNonPositiveIssue(
        List<MarketDataNormalizationIssue> issues,
        decimal value,
        MarketDataNormalizationIssueCode code,
        string message)
    {
        if (value <= 0)
        {
            issues.Add(new MarketDataNormalizationIssue(code, message));
        }
    }
}
