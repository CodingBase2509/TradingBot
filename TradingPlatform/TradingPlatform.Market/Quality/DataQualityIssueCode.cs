namespace TradingPlatform.Market.Quality;

internal enum DataQualityIssueCode
{
    InstrumentMismatch,
    PriceNotOnTickSize,
    QuantityNotOnMinimumStep,
    OutsideTradingSession,
    TimestampOutOfOrder,
    TimestampTooFarInFuture,
    StaleLiveData,
    DuplicateEvent,
    DuplicateSequenceNumber,
    SequenceOutOfOrder,
    SequenceGap,
    CrossedQuote,
    LockedQuote,
}
