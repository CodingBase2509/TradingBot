using NodaTime;
using TradingPlatform.Market.MarketData;

namespace TradingPlatform.Market.Quality;

internal sealed class DataQualityValidator : IDataQualityValidator
{
    private readonly DataQualityContext _context;
    private readonly IClock _clock;
    private readonly StreamState<Trade> _tradeState = new();
    private readonly StreamState<Quote> _quoteState = new();

    public DataQualityValidator(DataQualityContext context, IClock clock)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public DataQualityResult<Trade> Validate(Trade trade)
    {
        ArgumentNullException.ThrowIfNull(trade);

        var issues = new List<DataQualityIssue>();
        ValidateCommon(trade.InstrumentId, trade.OccurredAt, issues);
        ValidateStep(
            trade.Price,
            _context.Instrument.TickSize,
            DataQualityIssueCode.PriceNotOnTickSize,
            "The trade price is not aligned to the instrument tick size.",
            issues);
        ValidateStep(
            trade.Quantity,
            _context.Instrument.MinimumQuantity,
            DataQualityIssueCode.QuantityNotOnMinimumStep,
            "The trade quantity is not aligned to the instrument minimum quantity.",
            issues);
        ValidateStreamState(
            trade,
            trade.OccurredAt,
            trade.Origin.SequenceNumber,
            _tradeState,
            issues);

        return DataQualityResult<Trade>.Create(trade, issues);
    }

    public DataQualityResult<Quote> Validate(Quote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        var issues = new List<DataQualityIssue>();
        ValidateCommon(quote.InstrumentId, quote.OccurredAt, issues);
        ValidateStep(
            quote.BidPrice,
            _context.Instrument.TickSize,
            DataQualityIssueCode.PriceNotOnTickSize,
            "The bid price is not aligned to the instrument tick size.",
            issues);
        ValidateStep(
            quote.AskPrice,
            _context.Instrument.TickSize,
            DataQualityIssueCode.PriceNotOnTickSize,
            "The ask price is not aligned to the instrument tick size.",
            issues);
        ValidateStep(
            quote.BidQuantity,
            _context.Instrument.MinimumQuantity,
            DataQualityIssueCode.QuantityNotOnMinimumStep,
            "The bid quantity is not aligned to the instrument minimum quantity.",
            issues);
        ValidateStep(
            quote.AskQuantity,
            _context.Instrument.MinimumQuantity,
            DataQualityIssueCode.QuantityNotOnMinimumStep,
            "The ask quantity is not aligned to the instrument minimum quantity.",
            issues);

        if (quote.BidPrice > quote.AskPrice)
        {
            AddError(issues, DataQualityIssueCode.CrossedQuote, "The bid price is greater than the ask price.");
        }
        else if (quote.BidPrice == quote.AskPrice)
        {
            AddWarning(issues, DataQualityIssueCode.LockedQuote, "The bid and ask prices are equal.");
        }

        ValidateStreamState(
            quote,
            quote.OccurredAt,
            quote.Origin.SequenceNumber,
            _quoteState,
            issues);

        return DataQualityResult<Quote>.Create(quote, issues);
    }

    private void ValidateCommon(
        Platform.Identifiers.InstrumentId instrumentId,
        Instant occurredAt,
        List<DataQualityIssue> issues)
    {
        if (instrumentId != _context.Instrument.Id)
        {
            AddError(
                issues,
                DataQualityIssueCode.InstrumentMismatch,
                "The market data event does not belong to the configured instrument.");
        }

        if (!_context.Calendar.IsOpen(occurredAt))
        {
            AddError(
                issues,
                DataQualityIssueCode.OutsideTradingSession,
                "The market data event occurred outside a configured trading session.");
        }

        if (_context.Mode is DataQualityMode.Live)
        {
            var now = _clock.GetCurrentInstant();
            if (occurredAt > now + _context.MaxFutureSkew)
            {
                AddError(
                    issues,
                    DataQualityIssueCode.TimestampTooFarInFuture,
                    "The live market data timestamp is too far in the future.");
            }

            if (occurredAt < now - _context.MaxLiveDataAge)
            {
                AddError(
                    issues,
                    DataQualityIssueCode.StaleLiveData,
                    "The live market data event is too old.");
            }
        }
    }

    private static void ValidateStep(
        decimal value,
        decimal step,
        DataQualityIssueCode code,
        string message,
        List<DataQualityIssue> issues)
    {
        if (value % step != 0)
        {
            AddError(issues, code, message);
        }
    }

    private static void ValidateStreamState<T>(
        T value,
        Instant occurredAt,
        long? sequenceNumber,
        StreamState<T> state,
        List<DataQualityIssue> issues)
        where T : class
    {
        var isDuplicate = state.LastValue is not null &&
            EqualityComparer<T>.Default.Equals(state.LastValue, value);
        if (isDuplicate)
        {
            AddError(issues, DataQualityIssueCode.DuplicateEvent, "The market data event is an exact duplicate.");
        }

        if (state.LastOccurredAt is { } lastOccurredAt && occurredAt < lastOccurredAt)
        {
            AddError(
                issues,
                DataQualityIssueCode.TimestampOutOfOrder,
                "The market data timestamp is older than the latest observed timestamp.");
        }

        if (!isDuplicate && sequenceNumber is { } currentSequence && state.LastSequenceNumber is { } lastSequence)
        {
            if (currentSequence == lastSequence)
            {
                AddError(
                    issues,
                    DataQualityIssueCode.DuplicateSequenceNumber,
                    "The market data sequence number has already been observed.");
            }
            else if (currentSequence < lastSequence)
            {
                AddError(
                    issues,
                    DataQualityIssueCode.SequenceOutOfOrder,
                    "The market data sequence number is older than the latest observed sequence number.");
            }
            else if (currentSequence - lastSequence > 1)
            {
                AddWarning(
                    issues,
                    DataQualityIssueCode.SequenceGap,
                    "The market data sequence contains a gap.");
            }
        }

        if (state.LastOccurredAt is null || occurredAt > state.LastOccurredAt.Value)
        {
            state.LastOccurredAt = occurredAt;
        }

        if (sequenceNumber is { } sequence &&
            (state.LastSequenceNumber is null || sequence > state.LastSequenceNumber.Value))
        {
            state.LastSequenceNumber = sequence;
        }

        state.LastValue = value;
    }

    private static void AddWarning(
        List<DataQualityIssue> issues,
        DataQualityIssueCode code,
        string message) =>
        issues.Add(new DataQualityIssue(code, DataQualityIssueSeverity.Warning, message));

    private static void AddError(
        List<DataQualityIssue> issues,
        DataQualityIssueCode code,
        string message) =>
        issues.Add(new DataQualityIssue(code, DataQualityIssueSeverity.Error, message));

    private sealed class StreamState<T>
        where T : class
    {
        public T? LastValue { get; set; }

        public Instant? LastOccurredAt { get; set; }

        public long? LastSequenceNumber { get; set; }
    }
}
