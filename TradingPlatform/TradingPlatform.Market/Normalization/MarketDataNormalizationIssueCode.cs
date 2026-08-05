namespace TradingPlatform.Market.Normalization;

internal enum MarketDataNormalizationIssueCode
{
    MissingProvider,
    InvalidProvider,
    MissingSymbol,
    InvalidSymbol,
    InvalidTradePrice,
    InvalidTradeQuantity,
    InvalidBidPrice,
    InvalidBidQuantity,
    InvalidAskPrice,
    InvalidAskQuantity,
    InvalidSequenceNumber,
    UnknownInstrument,
}
