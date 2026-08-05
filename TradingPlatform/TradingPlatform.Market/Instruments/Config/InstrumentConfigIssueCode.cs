namespace TradingPlatform.Market.Instruments.Config;

internal enum InstrumentConfigIssueCode
{
    MissingMetadata,
    MissingInstrumentId,
    MissingName,
    UnknownInstrumentType,
    MissingExchange,
    MissingCurrency,
    InvalidTickSize,
    InvalidTickValue,
    InvalidMinimumQuantity,
    UnsupportedTradeDirection,
    MissingProviderSymbols,
    UnknownProviderKind,
    MissingProviderName,
    MissingProviderSymbol,
    MissingMarketDataSymbol,
    DuplicateProviderSymbol,
    MissingCalendar,
    InvalidCalendarId,
    MissingRolloverRule,
    InvalidRolloverRuleId,
    RolloverWithoutExpiringContracts,
    RolloverRuleWithoutRollover,
}
